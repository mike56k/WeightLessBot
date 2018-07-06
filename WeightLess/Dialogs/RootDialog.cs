using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace WeightLess.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        bool isKnown = false;
        bool isBegin = false;
        bool isStarted = false;
        string activity;
        UserFoloder.User user = new UserFoloder.User();

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;
            string input = message.Text.ToUpper();
            if (input.Length > 0)
            {
                isStarted = true;
            }

            if (!isKnown && input.Contains("ПРИВЕТ"))
            {
                await context.PostAsync($"Привет, {message.From.Name}!\n Если Вы еще не указывали свои данные, то напишите **начать**");
                isKnown = true;
            }
            else if (input.Contains("ПРИВЕТ") && isKnown)
            {
                await context.PostAsync("Здравствуй, но мы уже здоровались");
                context.Wait(MessageReceivedAsync);
            }
            else if (input.Contains("НАЧАТЬ") && !isBegin)
            {
                isBegin = true;
                PromptDialog.Choice(
                   context: context,
                   resume: SetGender,
                   options: new List<string> { "Мужcкой", "Женский" },
                   prompt: $"Укажите ваш пол:",
                   promptStyle: PromptStyle.Auto
                   );
            }
            else if (input.Contains("НАЧАТЬ") && isBegin)
            {
                await context.PostAsync("У меня уже есть данные о Вас");
                context.Wait(MessageReceivedAsync);
            }
            else if (input.Contains("СТАРТ") && isBegin)
            {
                PromptDialog.Choice(
                   context: context,
                   resume: ChooseAction,
                   options: new List<string> { "Рассчитать индекс массы тела",
                       "Рассчитать вес для нормализации индекса массы тела",
                       "Рассчитать дневную норму калорий",
                       "Посчитать потраченные калории",
                       "Сброс параметров" },
                   prompt: $"Бот WeightLess \nВыберите функцию:",
                   promptStyle: PromptStyle.Auto);
            }
            else if (input.Contains("ИМНФОРМ") && isBegin)
            {
                await context.PostAsync($"Рост {user.Height} \nВес {user.Weight}" +
                    $"\nВозраст {user.Age} \nИМТ {user.Bmi} \nТип {user.Type} \nСоответствие норме {user.InLimit}");
                context.Wait(MessageReceivedAsync);
            }
            else if (input.Contains("ИНФОРМ") && !isBegin)
            {
                await context.PostAsync("У меня нет информации о тебе. Пройди опрос по команде **начать**");
                context.Wait(MessageReceivedAsync);
            }
            else if (input.Contains("КАЛОРИИ") && isBegin)
            {
                await context.PostAsync($"Ваш пол - {user.Sex}\nВаш возраст - {user.Age}\nВы ведете {user.LifeStyle.ToLower()} образ жизни\nНа основе этого я " +
                    $"рекомендую потреблять вам примерно {user.SetDailyColories(user.LifeStyle, user.Age, user.Sex)} калорий в сутки");
                context.Wait(MessageReceivedAsync);
            }
            else if (input.Contains("КАЛОРИИ") && !isBegin)
            {
                await context.PostAsync("У меня нет информации о тебе. Пройди опрос по команде **начать**");
                context.Wait(MessageReceivedAsync);
            }
            else if (isBegin)
            {
                PromptDialog.Choice(
                   context: context,
                   resume: ChooseAction,
                   options: new List<string> { "Информация о пользователе",
                       "Рассчитать индекс массы тела",
                       "Рассчитать вес для нормализации индекса массы тела",
                       "Посчитать потраченные калории",
                       "Рассчитать дневную норму калорий",
                       "Сброс параметров" },
                   prompt: "Выберите функцию:",
                   promptStyle: PromptStyle.Auto);

                //await context.PostAsync("Чтобы начать напишите **начать**");
            }
            else
            {
                await context.PostAsync($"Если Вы еще не указали свои параметры напишите **начать**\nЕсли же Вы уже указали свои данные, то напишите **старт**");
                context.Wait(MessageReceivedAsync);
            }
        }

        public virtual async Task ChooseAction(IDialogContext context, IAwaitable<string> Action)
        {
            string input = await Action;
            if (input == "Информация о пользователе")
            {
                HeroCard card = new HeroCard
                {
                    Title = "Информация о пользователе",
                    Text = $" Пол: {user.Sex}" +
                    $" Рост: {user.Height} " +
                    $" Вес: {user.Weight.ToString("F1")}" +
                    $" Возраст: {user.Age} " +
                    $" ИМТ: {UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")} " +
                    $" Образ жизни: {user.LifeStyle}"
                };

                IMessageActivity message = context.MakeMessage();
                message.Attachments.Add(card.ToAttachment());

                await context.PostAsync(message);

                /*IMessageActivity mess = context.MakeMessage();
                ReceiptCard receipt = new ReceiptCard
                {
                    Title = "Информация о пользователе",
                    Facts = new List<Fact>
                    {
                        new Fact
                        {
                            Key = "Пол:",
                            Value = user.Sex
                        },
                        new Fact
                        {
                            Key = "Рост:",
                            Value = user.Height.ToString()
                        },
                        new Fact
                        {
                            Key = "Вес:",
                            Value = user.Weight.ToString("F1")
                        },
                        new Fact
                        {
                            Key = "Возраст:",
                            Value = user.Age.ToString()
                        },
                        new Fact
                        {
                            Key = "Индекс массы тела:",
                            Value = UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")
                        },
                        new Fact
                        {
                            Key = "Образ жизни:",
                            Value = user.LifeStyle
                        },
                },
                };
                mess.Attachments.Add(receipt.ToAttachment());
                await context.PostAsync(mess);*/
            }
            else if (input == "Рассчитать индекс массы тела")
            {
                await context.PostAsync($"Ваш индекс массы тела: {UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")}");
            }
            else if (input == "Рассчитать вес для нормализации индекса массы тела")
            {
                if (UserFoloder.BodyStats.BMI(user.Height, user.Weight) >= 18.5 &&
                  UserFoloder.BodyStats.BMI(user.Height, user.Weight) <= 24.99)
                {
                    await context.PostAsync($"Ваш индекс массы тела находится в пределах нормы\nТак держать!");
                }
                else if (UserFoloder.BodyStats.BMI(user.Height, user.Weight) >= 25.00)
                {
                    await context.PostAsync($"Ваш индес массы тела выше нормы" +
                        $"\nНорма: 18.5 - 24.99 \nВаш: {UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")}" +
                        $"\nДля достижении нормы ваш вес, при вашем росте, должен составить {((user.Height / 100.0) * (user.Height / 100.0) * 24.0).ToString("F1")}");
                }
                else if (UserFoloder.BodyStats.BMI(user.Height, user.Weight) <= 18.5)
                {
                    await context.PostAsync($"Ваш индекс массы тела ниже нормы" +
                        $"\nНорма: 18.5 - 24.99 \nВаш: {UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")}" +
                        $"\nДля достижении нормы ваш вес, при вашем росте, должен составить {((user.Height / 100.0) * (user.Height / 100.0) * 18.5).ToString("F1")}");
                }
            }
            else if (input == "Рассчитать дневную норму калорий")
            {
                await context.PostAsync($"Ваш пол: {user.Sex}\nВаш возраст: {user.Age}\nВы ведете {user.LifeStyle.ToLower()} образ жизни\nНа основе этого я " +
                       $"рекомендую потреблять вам примерно {user.SetDailyColories(user.LifeStyle, user.Age, user.Sex)} калорий в сутки");
            }
            else if (input == "Посчитать потраченные калории")
            {
                // Mike function
                PromptDialog.Choice(
                  context: context,
                  resume: ActivityChoice,
                  options: new List<string> { "Прогулка \n Медленная езда на велосипеде",
                      "Настольный теннис",
                      "Гимнастика",
                      "Роликовые коньки \n Большой теннис \n Бадбинтон",
                      "Бег трусцой \n Баскетбол \n Хоккей" },
                  prompt: $"Укажите вид активности. От этого зависит потребление калорий:",
                  promptStyle: PromptStyle.Auto
                  );
            }
            else if (input == "Сброс параметров")
            {
                PromptDialog.Choice(
                    context: context,
                    resume: AreYouSure,
                    options: new List<string> { "Да", "Нет" },
                    prompt: $"Вы уверены что хотите сбросить?",
                    retry: "Введите корректные данные"
                    );
            }

        }

        private async Task ActivityChoice(IDialogContext context, IAwaitable<string> result)
        {
            activity = (await result).ToString();
            PromptDialog.Text(
                context: context,
                resume: ActivityChoiceResult,
                prompt: "Укажите время (в минутах), в течении которого вы занимались этой активностью",
                retry: "Введите корректные данные" // для обхода вброшенных картинок, аудио и др.
            );
        }

        private async Task ActivityChoiceResult(IDialogContext context, IAwaitable<string> result)
        {
            int minutes = -1;
            try
            {
                minutes = Convert.ToInt32(await result);
            }
            catch (Exception e)
            { }

            if ((minutes <= 0))
            {
                PromptDialog.Text(
                    context: context,
                    resume: ActivityChoiceResult,
                    prompt: "Введите корректные данные"
                    );
            }
            else
            {

                double activitychoiceresult = 0.0;
                if (activity == "Прогулка \n Медленная езда на велосипеде")
                {
                    activitychoiceresult = 4.5 * minutes;
                }
                else if (activity == "Настольный теннис")
                {
                    activitychoiceresult = 5.5 * minutes;
                }
                else if (activity == "Гимнастика")
                {
                    activitychoiceresult = 6.5 * minutes;
                }
                else if (activity == "Роликовые коньки \n Большой теннис \n Бадбинтон")
                {
                    activitychoiceresult = 7.5 * minutes;
                }
                else if (activity == "Бег трусцой \n Баскетбол \n Хоккей")
                {
                    activitychoiceresult = 9.5 * minutes;
                }
                else
                {
                    PromptDialog.Text(
                        context: context,
                        resume: ActivityChoiceResult,
                        prompt: "Укажите время (в минутах), в течении которого вы занимались этой активностью",
                        retry: "Введите корректные данные" // для обхода вброшенных картинок, аудио и др.
                        );
                }
                await context.PostAsync($"Вы потратили {activitychoiceresult.ToString("F1")}  калорий!");
                context.Done(this);
            }
        }

        public virtual async Task SetGender(IDialogContext context, IAwaitable<string> UserGender)
        {
            string tmp = (await UserGender).ToString();
            if (tmp == "Мужcкой") user.Sex = "Мужской";
            else if (tmp == "Женский") user.Sex = "Женский";
            else PromptDialog.Choice(
                   context: context,
                   resume: SetGender,
                   options: new List<string> { "Мужcкой", "Женский" },
                   prompt: $"Укажите ваш пол:",
                   promptStyle: PromptStyle.Auto
                   );

            PromptDialog.Text(
                context: context,
                resume: SetHeight,
                prompt: "Укажите Ваш рост (в сантиметрах)",
                retry: "Введите корректные данные" // для обхода вброшенных картинок, аудио и др.
            );

        }
        public virtual async Task SetHeight(IDialogContext context, IAwaitable<string> UserHeight)
        {
            int tmp = 0;
            try
            {
                tmp = Convert.ToInt32(await UserHeight);
            }
            catch (Exception e)
            {
                //await context.PostAsync($"Введите корректные данные");
            }
            user.Height = tmp;

            if (user.Height == -1)
            {
                PromptDialog.Text(
                context: context,
                resume: SetHeight,
                prompt: "Укажите Ваш рост корректно"
                );
            }
            else
            {
                PromptDialog.Text(
                context: context,
                resume: SetWeight,
                prompt: "Укажите Ваш вес (в килограммах)"
                );
            }
        }

        public virtual async Task SetWeight(IDialogContext context, IAwaitable<string> UserWeight)
        {
            double tmp = -1;
            try
            {
                tmp = Convert.ToDouble((await UserWeight).ToString().Replace('.', ','));
            }
            catch (Exception e)
            {
                //await context.PostAsync($"Введите корректные данные");
            }
            user.Weight = tmp;

            if (user.Weight == -1)
            {
                //await context.PostAsync($"Введите корректные данные");
                PromptDialog.Text(
                context: context,
                resume: SetWeight,
                prompt: "Укажите Ваш вес корректно"
                );
            }
            else
            {
                PromptDialog.Text(
                    context: context,
                    resume: SetAge,
                    prompt: "Укажите Ваш возраст"
                    );
            }
        }

        public virtual async Task SetAge(IDialogContext context, IAwaitable<string> UserAge)
        {
            int tmp = -1;
            try
            {
                tmp = Convert.ToInt32(await UserAge);
            }
            catch (Exception e)
            {
                //await context.PostAsync($"Введите корректные данные");
            }

            user.Age = tmp;
            if (user.Age == -1)
            {
                //await context.PostAsync($"Введите корректные данные");
                PromptDialog.Text(
                    context: context,
                    resume: SetAge,
                    prompt: "Укажите Ваш возраст корректно"
                    );
            }
            else
            {
                PromptDialog.Choice(
                    context: context,
                    resume: SetStyle,
                    options: new List<string> { "Сидячий", "Умеренный", "Активный" },
                    prompt: $"Укажите свой образ жизни",
                    promptStyle: PromptStyle.Auto
               );
            }
        }
        public virtual async Task SetStyle(IDialogContext context, IAwaitable<string> style)
        {
            string input = await style;
            user.LifeStyle = input;

            /*IMessageActivity mess = context.MakeMessage();
            ReceiptCard receipt = new ReceiptCard
            {
                Title = "Информация о пользователе",
                Facts = new List<Fact>
                    {
                        new Fact
                        {
                            Key = "Пол:",
                            Value = user.Sex
                        },
                        new Fact
                        {
                            Key = "Рост:",
                            Value = user.Height.ToString()
                        },
                        new Fact
                        {
                            Key = "Вес:",
                            Value = user.Weight.ToString("F1")
                        },
                        new Fact
                        {
                            Key = "Возраст:",
                            Value = user.Age.ToString()
                        },
                        new Fact
                        {
                            Key = "Индекс массы тела:",
                            Value = UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")
                        },
                        new Fact
                        {
                            Key = "Образ жизни:",
                            Value = user.LifeStyle
                        },
                },
            };
            mess.Attachments.Add(receipt.ToAttachment());
            await context.PostAsync(mess);
            */
            HeroCard card = new HeroCard
            {
                Title = "Информация о пользователе",
                Text = $" Пол: {user.Sex}" +
                    $" Рост: {user.Height} " +
                    $" Вес: {user.Weight.ToString("F1")}" +
                    $" Возраст: {user.Age} " +
                    $" ИМТ: {UserFoloder.BodyStats.BMI(user.Height, user.Weight).ToString("F1")} " +
                    $" Образ жизни: {user.LifeStyle}"
            };

            IMessageActivity message = context.MakeMessage();
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);
            string type = UserFoloder.BodyStats.Type(UserFoloder.BodyStats.BMI(user.Height, user.Weight), user.Age).ToString();
            string equivalence = UserFoloder.BodyStats.InLmit(user.Age, UserFoloder.BodyStats.BMI(user.Height, user.Weight));
            await context.PostAsync($"Классификация: {type} \nСоответствие норме: {equivalence}");
            await context.PostAsync($"Для использования функций бота напишите любую последовательность символов");

            context.Done(this);

        }

        public virtual async Task AreYouSure(IDialogContext context, IAwaitable<string> Sure)
        {
            string tmp = await Sure;
            if (tmp == "Да")
            {
                isBegin = false;
                await context.PostAsync($"Я удалил все данные о вас \nЧтобы начать напишите **начать**");
            }
            if (tmp == "Нет")
            {
                await context.PostAsync($"Вы решили не удалять данные");
            }

            context.Done(this);
        }
    }
}


