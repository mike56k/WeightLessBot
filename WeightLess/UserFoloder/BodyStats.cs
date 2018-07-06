using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeightLess.UserFoloder
{
    [Serializable]
    public static class BodyStats
    {
        public static double BMI(int height, double weght)
        {
            return (weght / ((height / 100.0) * (height / 100.0)));
        }

        public static string Type(double bmi, int age)
        {
            if (age >= 19)
            {
                if (bmi > 0.0 && bmi < 16.0) return "Выраженный дифицит";
                else if (bmi >= 16.0 && bmi <= 18.49) return "Недостаточная масса тела";
                else if (bmi >= 18.5 && bmi <= 24.99) return "Норма";
                else if (bmi >= 25.0 && bmi <= 29.99) return "Предожирение";
                else if (bmi >= 30.0 && bmi <= 34.99) return "Ожирение первой степени";
                else if (bmi >= 35.0 && bmi <= 39.99) return "Ожирение второй степени";
                else if (bmi > 40.0 && bmi <= 50) return "Ожирение третьей степени";
                else return "Невозможно определить. Были введены некорректные данные";
            }
            else
                return "Не подходит по возрастной группе (только с 19)";
        }

        public static string InLmit(int age, double bmi)
        {
            if ((age > 0 && age <= 3) && (bmi >= 14.0 && bmi <= 18.0)) return "Норма";
            else if ((age >= 4 && age <= 7) && (bmi >= 15.0 && bmi <= 19.0)) return "Норма";
            else if ((age >= 8 && age <= 12) && (bmi >= 14.0 && bmi <= 22.0)) return "Норма";
            else if ((age >= 13 && age <= 18) && (bmi >= 19.0 && bmi <= 23.0)) return "Норма";
            else if ((age >= 19 && age <= 24) && (bmi >= 19.0 && bmi <= 24.0)) return "Норма";
            else if ((age >= 25 && age <= 34) && (bmi >= 20.0 && bmi <= 25.0)) return "Норма";
            else if ((age >= 35 && age <= 44) && (bmi >= 21.0 && bmi <= 26.0)) return "Норма";
            else if ((age >= 45 && age <= 54) && (bmi >= 22.0 && bmi <= 27.0)) return "Норма";
            else if ((age >= 55 && age <= 64) && (bmi >= 23.0 && bmi <= 28.0)) return "Норма";
            else if ((age >= 65) && (bmi >= 24.0 && bmi <= 29.0)) return "Норма";
            return "Отклонение от нормы";
        }
    }
}