using System.ComponentModel.Design;
using System.Text;
using System.Transactions;

class Laba2
{
    public static void generation(int N, int L, int diap) // генерация
    {
        decimal[][] matrix = new decimal[N][];
        Random rnd = new Random();
        for(int i = 0; i < N; i++) // генерация
        {
            matrix[i]= new decimal[Math.Min(L,N-i)];
            matrix[i][0] = rnd.Next(diap);
            while(matrix[i][0]==0)
                matrix[i][0] = (decimal)(rnd.Next(diap*1000)/1000.0);
            for (int j = 1; j < Math.Min(L, N - i); j++)
                    matrix[i][j] = (decimal)(rnd.Next(2*diap*1000)/1000.0-diap);
        }

        for(int i = N-1; i >=0; i--) //перемножение
            for(int j = Math.Min(L-1,N-1-i); j >= 0; j--)
                for(int ki = 0, kj = j, k = i; k >= 0 && kj < L; k--, kj++, ki++)
                {
                    if(ki!=0)
                        matrix[i][j] += matrix[k][ki]*matrix[k][kj];
                    else
                        matrix[i][j] = matrix[k][ki]*matrix[k][kj];
                }

        decimal[] x = new decimal[N]; // генерация решения
        for(int i = 0; i < N; i++)
            x[i] = (decimal)(rnd.Next(diap*2*1000)/1000.0-diap);

        decimal[] f = new decimal[N]; // составление f
        for(int i = 0; i < N; i++)
        {
            f[i] = 0;
            for(int j = 0; j < Math.Min(L, N-i);j++)
                f[i]+=x[i+j]*matrix[i][j];
            for(int ki = 1, k = i-1; ki < L && k >= 0; ki++,k--)
                f[i]+=x[k]*matrix[k][ki];
        }
        using(StreamWriter sw = new StreamWriter("data.txt")) // можешь тут путь указать тут запись
        {
            sw.Write($"{N} {L}\n");
            for(int i = 0; i < N; i++)
            {
                for(int j = 0; j < Math.Min(L, N-i); j++)
                   { 
                    sw.Write(string.Format("{0:F40}", matrix[i][j]));
                    sw.Write(' ');
                   }
                sw.Write("\n");
            }
            for(int i = 0; i < N; i++)
                {
                    sw.Write(string.Format("{0:F40}", f[i]));
                    sw.Write(' ');
                }
            sw.Write("\n");
            for(int i = 0; i < N; i++)
                {
                    sw.Write(string.Format("{0:F40}", x[i]));
                    sw.Write(' ');
                }
            sw.Write("\n");
            sw.Close();
        }
    }
    public static bool solution() // решение
    {
        using (StreamReader sr = new StreamReader("data.txt")) // можешь тут путь указать
        {
            string nums = sr.ReadLine(); // чтение с файла
            string[] nums_arr = nums.Split(' ');
            int N, L;
            N = int.Parse(nums_arr[0]);
            L = int.Parse(nums_arr[1]);

            decimal[][] matrix = new decimal[N][];  // матрица
            for (int i = 0; i < N; i++)
            {
                matrix[i] = new decimal[Math.Min(L, N - i)];
                nums = sr.ReadLine();
                nums_arr = nums.Split();
                for (int j = 0; j < Math.Min(L, N - i); j++)
                    matrix[i][j] = decimal.Parse(nums_arr[j]);
            }

            decimal[] f = new decimal[N]; // f 
            nums = sr.ReadLine();
            nums_arr = nums.Split(' ');
            for (int i = 0; i < N; i++)
                f[i] = decimal.Parse(nums_arr[i]);

            decimal[] x_t = new decimal[N]; // иксы для оценки погрешности
            nums = sr.ReadLine();
            nums_arr = nums.Split(' ');
            for (int i = 0; i < N; i++)
               x_t[i] = decimal.Parse(nums_arr[i]);

            bool flag = true;
            for (int i = 0; i < N && flag; i++)
            {
                for (int ki = i - 1, kj = 1; ki >= 0 && kj < L; ki--, kj++) // преобразование матрицы методом квадратного корня
                    matrix[i][0] -= matrix[ki][kj] * matrix[ki][kj];
                if (matrix[i][0] < 0)
                    flag = false;
                else if (flag)
                {
                    matrix[i][0] = (decimal)Math.Sqrt((double)matrix[i][0]);
                    for (int j = 1; j < Math.Min(L, N - i); j++)
                    {
                        for (int k = i - 1, ki = 1, kj = j + 1; k >= 0 && kj < L; k--, kj++, ki++)
                            matrix[i][j] -= matrix[k][ki] * matrix[k][kj];
                        matrix[i][j] /= matrix[i][0];
                    }
                }
            }
            if (flag)
            {
                for (int i = 0; i < N; i++)
                {
                    for (int ki = i - 1, kj = 1; ki >= 0 && kj < L; ki--, kj++) // верхнетреугольная матрица * у = f
                        f[i] -= f[ki] * matrix[ki][kj]; // в f будут y 
                    f[i] /= matrix[i][0];
                }
                for (int i = N - 1; i >= 0; i--)
                {
                    for (int j = i + 1; j < Math.Min(L+i, N); j++) // нижнетреугольная матрица * x = y
                        f[i] -= f[j] * matrix[i][j - i];
                    f[i] /= matrix[i][0];  // в f будут x 
                }
            }
            using (StreamWriter sw = new StreamWriter("result.txt")) // можешь тут путь указать
            {
                if (flag) // запись в файл
                {
                    for (int i = 0; i < N; i++)
                    {
                        sw.Write(string.Format("{0:F40}",f[i]));
                        sw.Write(' ');
                    }
                    sw.Write("\n");
                    for (int i = 0; i < N; i++)
                    {
                        sw.Write(string.Format("{0:F40}",x_t[i]));
                        sw.Write(' ');
                    }
                }
                else
                    sw.WriteLine("error");
                sw.Close();
            }
            sr.Close();
            return flag;
        }
    }
   
   public static void acc_test(int N, int L, int diap)
   {
    do
        generation(N,L,diap);
    while(!solution());
    using(StreamReader sr = new StreamReader("result.txt"))
    {
        decimal[] x = new decimal[N]; // считываем что получилось
        string nums = sr.ReadLine();
        string[] nums_arr = nums.Split(' ');
        for(int i = 0; i < N; i++)
            x[i]= decimal.Parse(nums_arr[i]);

        decimal[] x_a = new decimal[N]; // считываем что должно быть
        nums = sr.ReadLine();
        nums_arr = nums.Split(' ');
        for(int i = 0; i < N; i++)
            x_a[i]= decimal.Parse(nums_arr[i]);
        sr.Close();
        decimal acc = 0;
        for(int i = 0; i < N; i++) // считаем погрешность
            acc = Math.Max(acc,Math.Abs(x[i]-x_a[i])/Math.Max(Math.Abs(x_a[i]),(decimal)0.01));
        using(StreamWriter sw = new StreamWriter("acc_test.txt",true))
            {
                sw.WriteLine($"{N} {L} {diap} {string.Format("{0:F40}",acc)}");
                sw.Close();
            }
    }
   }
   public static void Main(String[] args)
    {
        acc_test(100,20,10);
    }
}
