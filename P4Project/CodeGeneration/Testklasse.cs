using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class Program__
{
    public static string GetString_()
    {
        return Console.ReadLine();
    }

    public static float GetNumber_()
    {
        float val;
        while (!float.TryParse(Console.ReadLine(), out val)) ;

        return val;
    }

    public static void Print_(object txt)
    {
        Console.Write(txt);
    }

    public static int ChooseOption_(bool displayIndex, params string[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            string msg = ((displayIndex == true) ? $"{ i + 1}: " : "");
            Console.WriteLine(msg + $"{options[i]}");
        }

        int choice = 0;
        do
        {
            int.TryParse(Console.ReadLine(), out choice);
            if (choice <= 0 || choice > options.Length)
            {
                Console.WriteLine("Choice out of bounds. Please try again.");
            }
        }
        while (choice <= 0 || choice > options.Length);

        return choice;
    }

    public static float GetRandomFloat_(float min, float max, bool repeatable)
    {
        if (repeatable)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
        Random rnd = new Random();
        return (float)rnd.NextDouble() * (max - min) + min;
    }

    private static Random random;

    public static int GetRandomInt_(int min, int max, bool repeatable)
    {
        if (repeatable)
        {
            return random.Next(min, max + 1);
        }
        Random rnd = new Random();
        return rnd.Next(min, max + 1);
    }

    public static void SetSeed_(int seed)
    {
        random = new Random(seed);
    }

    private static void ListAdd_<T>(List<T> lst, T element)
    {
        lst.Add(element);
    }

    private static void ListRemove_<T>(List<T> lst, T element)
    {
        lst.Remove(element);
    }

    private static void ListEmpty_<T>(List<T> lst)
    {
        lst.Clear();
    }

    private static int ListLength_<T>(List<T> lst)
    {
        return lst.Count;
    }
    public class Colomn_
    {
        public List<string> Fields_ = new List<string>();
        public Colomn_()
        {
        }

    }
    public static void Main()
    {
        int numberOfRows_ = 10;
        int numberOfColomns_ = 10;
        int Antalbomber_ = 10;
        List<Colomn_> map_ = CreateField_(numberOfColomns_, numberOfRows_, Antalbomber_);
        PlaceNumbers_(map_);
        PrintMap_(map_);

    }

    public static List<Colomn_> CreateField_(int numberOfColomns_, int numberOfRows_, int numberOfBombs_)
    {
        int i_ = 0;
        int j_ = 0;
        List<Colomn_> colomnArray_ = new List<Colomn_>();
        while ((bool)(i_ < numberOfColomns_))
        {
            Colomn_ newColomn_ = new Colomn_();
            while ((bool)(j_ < numberOfRows_))
            {
                string field_ = (0).ToString();
                ListAdd_(newColomn_.Fields_, field_);
                j_ = (int)(j_ + 1);

            }
            ListAdd_(colomnArray_, newColomn_);
            i_ = (int)(i_ + 1);
            j_ = 0;

        }
        PlaceBombs_(numberOfBombs_, colomnArray_, numberOfColomns_, numberOfRows_);
        return colomnArray_;
    }

    public static void PlaceBombs_(int numberOfBombs_, List<Colomn_> map_, int numberOfColomns_, int numberOfRows_)
    {
        int i_ = 0;
        int row_ = 0;
        int colomn_ = 0;
        List<Colomn_> maps_ = map_;
        while ((bool)(i_ < numberOfBombs_))
        {
            row_ = GetRandomInt_(0, (int)(numberOfRows_ - 1), false);
            colomn_ = GetRandomInt_(0, (int)(numberOfColomns_ - 1), false);
            Print_(("i = ").ToString() + ((i_).ToString() + ((" row = ").ToString() + ((row_).ToString() + ((" colomn =  ").ToString() + ((colomn_).ToString() + ("\n").ToString()).ToString()).ToString()).ToString()).ToString()).ToString());
            if ((bool)(maps_[colomn_].Fields_[row_] != "x"))
            {
                i_ = (int)(i_ + 1);
                maps_[colomn_].Fields_[row_] = ("x").ToString();

            }

        }

    }

    public static void PlaceNumbers_(List<Colomn_> map_)
    {
        List<Colomn_> maps_ = map_;
        int colomnsNumbers_ = ListLength_(maps_);
        int rowsNumbers_ = ListLength_(maps_[0].Fields_);
        int i_ = 0;
        int j_ = 0;
        while ((bool)(i_ < colomnsNumbers_))
        {
            while ((bool)(j_ < rowsNumbers_))
            {
                if ((bool)(maps_[i_].Fields_[j_] == "x"))
                {
                    if ((bool)(i_ == 0))
                    {
                        if ((bool)(j_ == 0))
                        {
                            InsertNumber_((int)(i_ + 1), j_, maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ + 1), maps_);

                        }
                        else if ((bool)(j_ == rowsNumbers_))
                        {
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), j_, maps_);

                        }
                        else
                        {
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), j_, maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ + 1), maps_);

                        }

                    }
                    else if ((bool)(i_ == colomnsNumbers_))
                    {
                        if ((bool)(j_ == 0))
                        {
                            InsertNumber_((int)(i_ - 1), j_, maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ + 1), maps_);

                        }
                        else if ((bool)(j_ == rowsNumbers_))
                        {
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), j_, maps_);

                        }
                        else
                        {
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), j_, maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ + 1), maps_);

                        }

                    }
                    else
                    {
                        if ((bool)(j_ == 0))
                        {
                            InsertNumber_((int)(i_ - 1), j_, maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ + 1), maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ + 1), maps_);
                            InsertNumber_((int)(i_ + 1), j_, maps_);

                        }
                        else if ((bool)(j_ == rowsNumbers_))
                        {
                            InsertNumber_((int)(i_ - 1), j_, maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ - 1), maps_);
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ + 1), j_, maps_);

                        }
                        else
                        {
                            InsertNumber_(i_, (int)(j_ + 1), maps_);
                            InsertNumber_(i_, (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ - 1), maps_);
                            InsertNumber_((int)(i_ - 1), j_, maps_);
                            InsertNumber_((int)(i_ - 1), (int)(j_ + 1), maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ + 1), maps_);
                            InsertNumber_((int)(i_ + 1), j_, maps_);
                            InsertNumber_((int)(i_ + 1), (int)(j_ - 1), maps_);

                        }

                    }

                }
                j_ = (int)(j_ + 1);

            }
            j_ = 0;
            i_ = (int)(i_ + 1);

        }

    }

    public static void InsertNumber_(int colomn_, int row_, List<Colomn_> map_)
    {
        List<Colomn_> maps_ = map_;
        if ((bool)(maps_[colomn_].Fields_[row_] == "0"))
        {
            maps_[colomn_].Fields_[row_] = ("1").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "1"))
        {
            maps_[colomn_].Fields_[row_] = ("2").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "2"))
        {
            maps_[colomn_].Fields_[row_] = ("3").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "3"))
        {
            maps_[colomn_].Fields_[row_] = ("4").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "4"))
        {
            maps_[colomn_].Fields_[row_] = ("5").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "5"))
        {
            maps_[colomn_].Fields_[row_] = ("6").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "6"))
        {
            maps_[colomn_].Fields_[row_] = ("7").ToString();

        }
        else if ((bool)(maps_[colomn_].Fields_[row_] == "7"))
        {
            maps_[colomn_].Fields_[row_] = ("8").ToString();

        }

    }

    public static void PrintMap_(List<Colomn_> map_)
    {
        List<Colomn_> maps_ = map_;
        Print_("\n\n\n\n\n\n\n");
        int colomnsNumbers_ = ListLength_(maps_);
        int rowsNumbers_ = ListLength_(maps_[0].Fields_);
        int i_ = 0;
        int j_ = 0;
        while ((bool)(i_ < colomnsNumbers_))
        {
            while ((bool)(j_ < rowsNumbers_))
            {
                Print_((maps_[i_].Fields_[j_]).ToString() + (" ").ToString());
                j_ = (int)(j_ + 1);

            }
            Print_("\n");
            j_ = 0;
            i_ = (int)(i_ + 1);

        }

    }


}
