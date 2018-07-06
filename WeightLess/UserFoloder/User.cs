
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeightLess.UserFoloder
{
    [Serializable]
    public class User
    {
        string sex;
        int height;
        double weight;
        int age;
        double bmi;

        string type;
        string inLimit;
        string lifeStyle;

        //public User(string sex, int height, double weight, int age)
        //{
        //    this.sex = sex;
        //    this.height = height;
        //    this.weight = weight;
        //    this.age = age;
        //    bmi = BodyStats.BMI(height, weight);
        //    type = BodyStats.Type(bmi);
        //    inLimit = BodyStats.InLmit(age, bmi);
        //}

        public string Sex
        {
            set
            {
                sex = value;
            }
            get
            {
                return sex;
            }
        }

        public int Height
        {
            set
            {
                try
                {
                    if (value <= 80 || value >= 250)
                    {
                        height = -1;
                    }
                    else
                    {
                        height = value;
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            get
            {
                return height;
            }

        }

        public double Weight
        {
            set
            {
                try
                {
                    if (value <= 20 || value >= 500)
                    {
                        weight = -1;
                    }
                    else
                    {
                        weight = value;
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            get
            {
                return weight;
            }
        }

        public int Age
        {
            set
            {
                try
                {
                    if (value <= 5 || value > 120)
                    {
                        age = -1;
                    }
                    else
                    {
                        age = value;
                    }
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                }
            }
            get
            {
                return age;
            }
        }

        public double Bmi
        {
            set
            {
                bmi = BodyStats.BMI(height, weight);
            }
            get
            {
                return bmi;
            }
        }

        public string Type { get { return type; } }

        public string InLimit { get { return inLimit; } }

        public string LifeStyle
        {
            set
            {
                lifeStyle = value;
            }
            get
            {
                return lifeStyle;
            }
        }

        public int SetDailyColories(string lifeStyle, int age, string sex)
        {
            if (sex == "Мужской")
            {
                if (lifeStyle == "Сидячий")
                {
                    if (age < 19)
                        return 2500;
                    else if (age >= 19 && age <= 30)
                        return 2400;
                    else if (age >= 31 && age <= 50)
                        return 2200;
                    else
                        return 2000;
                }
                else if (lifeStyle == "Умеренный")
                {
                    if (age < 19)
                        return 2800;
                    else if (age >= 19 && age <= 30)
                        return 2700;
                    else if (age >= 31 && age <= 50)
                        return 2500;
                    else
                        return 2300;
                }
                else if (lifeStyle == "Активный")
                {
                    if (age < 19)
                        return 3200;
                    else if (age >= 19 && age <= 30)
                        return 3000;
                    else if (age >= 31 && age <= 50)
                        return 2900;
                    else
                        return 2600;
                }
                else
                    return -1;
            }
            else if (sex == "Женский")
            {
                if (lifeStyle == "Сидячий")
                {
                    if (age < 19)
                        return 1800;
                    else if (age >= 19 && age <= 25)
                        return 2000;
                    else if (age >= 26 && age <= 50)
                        return 1800;
                    else
                        return 1600;
                }
                else if (lifeStyle == "Умеренный")
                {
                    if (age < 19)
                        return 2000;
                    else if (age >= 19 && age <= 25)
                        return 2200;
                    else if (age >= 26 && age <= 50)
                        return 2200;
                    else
                        return 1800;
                }
                else if (lifeStyle == "Активный")
                {
                    if (age < 19)
                        return 2200;
                    else if (age >= 19 && age <= 30)
                        return 2400;
                    else if (age >= 31 && age <= 60)
                        return 2200;
                    else
                        return 2000;
                }
                else
                    return -1;
            }
            else
                return -1;
        }
    }
}
