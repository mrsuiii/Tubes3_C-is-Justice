using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class LCS
    {
        private int[][] dp;

        public LCS() { }

        public int _lcs(string x, string y, int m, int n)
        {
            // Inisialisasi dp array
            dp = new int[m + 1][];
            for (int i = 0; i <= m; i++)
            {
                dp[i] = new int[n + 1];
            }

            // Mengisi dp array
            for (int i = 0; i <= m; i++)
            {
                for (int j = 0; j <= n; j++)
                {
                    if (i == 0 || j == 0)
                    {
                        dp[i][j] = 0;
                    }
                    else if (x[i - 1] == y[j - 1])
                    {
                        dp[i][j] = dp[i - 1][j - 1] + 1;
                    }
                    else
                    {
                        dp[i][j] = Math.Max(dp[i - 1][j], dp[i][j - 1]);
                    }
                }
            }
            
            // Cetak panjang LCS
            return dp[m][n];
        }
    }
}
