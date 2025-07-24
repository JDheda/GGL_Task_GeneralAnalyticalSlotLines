using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
namespace Calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* ================================================================================================================== */
            /*                              General Analytical Slot Lines Game RTP Calculator                                     */
            /* ================================================================================================================== */

            /* General Analytical Slot Lines Game Calculator
             * Author:      Joshiro Dheda
             * Company:     Games Global
             * 
             * Features:
             * - up to 8 visible reels
             * - any visible reel-height
             * - visible reels-heights can vary
             * - wild substitution
             * - different types of wilds
             * - wilds can have multiplier when substituted
             * - scatters
             * - different types of scatters
             * - different scatters can trigger different features
             * - free-spins
             * - free-spin trigger and retrigger
             * - free-spin multipliers
             * 
             * All rights reserverd
             */

            /* ================================================================================================================== */
            /*                                                Game Definition                                                     */
            /* ================================================================================================================== */

            int[] nRows = { 3, 3, 3, 3, 3 };
            int nCols = nRows.Length;
            int nPayLines = 20;
            double coinsPerLine = 1;

            bool[] freeSpins = { false };
            int[] freeSpinTrigger = { 3 };
            bool scatter = true;
            bool bonus = true;
            int[] nFreeSpins = { 0 };
            bool[] freeSpinRetrigger = { false };
            bool wild = true;
            int[] freeSpinMultiplier = { 0 };
            double[] pureWildSubMultiplier = { 1 };
            double mixedWildSubMultiplier = 1;

            string[] reelXSymbols = { "High1", "High2", "High3", "High4", "Low1", "Low2", "Low3", "Low4" };
            string[] reelWSymbols = { "Wild" };
            string[] reelSSymbols = { "Scatter" };
            string[] reelBSymbols = { "Bonus1", "Bonus2" };

            // gets transposed. defined this way because of length
            double[,] payXTable = { { 0, 0, 0, 0, 0, 0, 0, 0 },
                                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                                    { 20, 18, 16, 14, 10, 8, 7, 6 },
                                    { 40, 36, 32, 28, 20, 16, 14, 12 },
                                    { 120, 108, 96, 84, 60, 48, 42, 36 } };

            // do not transpose
            double[,] payWTable = { { 0, 10, 50, 300, 5000 } };

            // make this one a vector
            double[] payWMixedTable = { 0, 0, 0, 0, 0 };

            // do not transpose
            double[,] paySTable = { { 0, 0, 5, 20, 600 } };

            // initial is for 0 bonuses
            double[,] payBTable = { { 0, 1, 2, 4, 8, 16, 32, 64, 128, 256 }, { 0, 2, 4, 8, 16, 32, 64, 128, 256, 512 } };

            string[] reel1 = { "Wild", "Wild", "Wild", "High1", "High1", "High1", "Low4", "Low4", "Low4", "Bonus1", "Bonus1", "High2", "High2", "High2", "Low3", "Low3", "Low3", "High3", "High3", "High3", "Scatter", "Low2", "Low2", "Low2", "High4", "High4", "High4", "Bonus2", "Low1", "Low1", "Low1", "High1", "High1", "High1", "High3", "High3", "High3", "Low2", "Low2", "Low2", "Low4", "Low4", "Low4" };
            string[] reel2 = { "Wild", "Wild", "Wild", "Low1", "Low1", "Low1", "High4", "High4", "High4", "Bonus1", "Low2", "Low2", "Low2", "High3", "High3", "High3", "Low3", "Low3", "Low3", "Scatter", "High2", "High2", "High2", "Low1", "Low1", "Low1", "Bonus2", "High1", "High1", "High1", "High2", "High2", "High2", "High4", "High4", "High4", "Low3", "Low3", "Low3", "Low4", "Low4", "Low4" };
            string[] reel3 = { "Wild", "Wild", "Wild", "High1", "High1", "High1", "Low4", "Low4", "Low4", "Bonus1", "High2", "High2", "High2", "Scatter", "Low3", "Low3", "Low3", "High3", "High3", "High3", "Scatter", "Low2", "Low2", "Low2", "Bonus2", "High4", "High4", "High4", "Low1", "Low1", "Low1", "Low4", "Low4", "Low4", "Low2", "Low2", "Low2", "Low4", "Low4", "Low4", "Low2", "Low2", "Low2" };
            string[] reel4 = { "Wild", "Wild", "Wild", "Low1", "Low1", "Low1", "High4", "High4", "High4", "Bonus1", "Low2", "Low2", "Low2", "High3", "High3", "High3", "Low3", "Low3", "Low3", "Scatter", "High2", "High2", "High2", "Low1", "Low1", "Low1", "Bonus2", "High1", "High1", "High1", "Scatter", "High2", "High2", "High2", "High4", "High4", "High4", "Low3", "Low3", "Low3", "Low4", "Low4", "Low4" };
            string[] reel5 = { "Wild", "Wild", "Wild", "Wild", "Low1", "Low1", "Low1", "High4", "High4", "High4", "Bonus1", "Bonus1", "Scatter", "Low2", "Low2", "Low2", "High3", "High3", "High3", "Low3", "Low3", "Low3", "Scatter", "High2", "High2", "High2", "Bonus2", "Low4", "Low4", "Low4", "High1", "High1", "High1", "Scatter", "High3", "High3", "High3", "High1", "High1", "High1" };

            string[][] reels = { reel1, reel2, reel3, reel4, reel5 };
            int[] nReel = ReelTotals(reels);

            /* ================================================================================================================== */
            /*                                                Main Calculations                                                   */
            /* ================================================================================================================== */

            string[][] virtualReels = ExpandColumns(reels, nRows);
            Vector<double> nRowsVec = Vector<double>.Build.DenseOfArray(nRows.Select(x => (double)x).ToArray());
            double betCost = nPayLines * coinsPerLine;
            int nXSymbols = reelXSymbols.Length;
            int nWSymbols = reelWSymbols.Length;
            int nSSymbols = reelSSymbols.Length;
            int nBSyambols = reelBSymbols.Length;
            Matrix<double> payXMat = Matrix<double>.Build.DenseOfArray(payXTable).Transpose();
            int R = payXMat.RowCount;
            int C = payXMat.ColumnCount;
            Matrix<double> payWMat = Matrix<double>.Build.DenseOfArray(payWTable);
            Vector<double> payWMixedVec = Vector<double>.Build.DenseOfArray(payWMixedTable);
            Matrix<double> payWMixedMat = payWMixedVec.ToRowMatrix();
            Matrix<double> paySMat = Matrix<double>.Build.DenseOfArray(paySTable);
            Vector<double> nReelVec = Vector<double>.Build.DenseOfArray(nReel.Select(x => (double)x).ToArray());
            Matrix<double> nXInReel = HowManyOfEachSymbolInReel(reelXSymbols, reels);
            Matrix<double> nWInReel = HowManyOfEachSymbolInReel(reelWSymbols, reels);
            Matrix<double> nSInReel = HowManyOfEachSymbolInReel(reelSSymbols, reels);
            Matrix<double> nBInReel = HowManyOfEachSymbolInReel(reelBSymbols, reels);
            Console.WriteLine("nWInReel: " + nWInReel);
            Console.WriteLine("nXInReel: " + nXInReel);
            Console.WriteLine("nSInReel: " + nSInReel);
            Console.WriteLine("nBInReel: " + nBInReel);

            Matrix<double> xStreakWithoutWSub = XStreakWithoutWildSub(nXInReel, nWInReel, nSInReel, nReel, wild);
            // Matrix<double> wStreakWithoutWSub = WStreakWithoutWildSub(nWInReel, nSInReel, nReel, scatter, mixedWild);
            Matrix<double> wPureTypeStreakWithoutWSub = WPureTypeStreakWithoutWildSub(nWInReel, nSInReel, nBInReel, nReel, scatter, bonus);
            Vector<double> wMixedStreakWithoutWSub = WMixedStreakWithoutWildSub(nWInReel, nSInReel, nBInReel, nReel, scatter, bonus);
            Matrix<double> xStreakWithWSub = XStreakWithWildSub(nXInReel, nWInReel, nSInReel, nReel, wild);
            Matrix<double> xFollowsWStreak = XFollowsWStreak(nXInReel, nWInReel, nSInReel, nReel, wild);
            // Matrix<double> xFollowsPureW1Streak = XFollowsPureWStreak(nXInReel, nWInReel, nSInReel, nReel, wild, 0);
            // Matrix<double> wStreakFollowedByY = WildStreakFollowedByY(nXInReel, nWInReel, nSInReel, nReel, wild);
            Matrix<double> wStreakFollowedByNotXW = WildStreakFollowedByNotXW(nXInReel, nWInReel, nSInReel, nReel, wild);
            // Matrix<double> pureWStreakFollowedByNotXW = PureWildStreakFollowedByNotXW(nXInReel, nWInReel, nSInReel, nReel, wild, 0);
            // Matrix<double> wStreakFollowedByXThen1XW = WildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, 1);
            // Matrix<double> wStreakFollowedByXThen2XW = WildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, 2);
            // Matrix<double> wStreakFollowedByXThen3XW = WildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, 3);

            // double[,] scatterCombinations = ScatterCombinationsGenerator(5, 2);
            // Matrix<double> scatterCombinationsMat = Matrix<double>.Build.DenseOfArray(scatterCombinations);

            // Console.WriteLine("xStreakWithoutWSub: " + xStreakWithoutWSub);
            // Console.WriteLine("wStreakWithoutWSub: " + wStreakWithoutWSub);
            Console.WriteLine("wPureTypeStreakWithoutWSub: " + wPureTypeStreakWithoutWSub);
            Console.WriteLine("wMixedStreakWithoutWSub: " + wMixedStreakWithoutWSub);
            Console.WriteLine("xStreakWithWSub: " + xStreakWithWSub);
            Console.WriteLine("xFollowsWStreak: " + xFollowsWStreak);
            // Console.WriteLine("xFollowsPureW1Streak: " + xFollowsPureW1Streak);
            // Console.WriteLine("wStreakFollowedByY: " + wStreakFollowedByY);
            Console.WriteLine("wStreakFollowedByNotXW: " + wStreakFollowedByNotXW);
            // Console.WriteLine("wStreakFollowedByXThen1XW: " + wStreakFollowedByXThen1XW);
            // Console.WriteLine("wStreakFollowedByXThen2XW: " + wStreakFollowedByXThen2XW);
            // Console.WriteLine("wStreakFollowedByXThen3XW: " + wStreakFollowedByXThen3XW);
            // -------------------------------------------------------------------------------------------------------------------
            // Interesting Logic For Main Calcs Begins Here....
            // -------------------------------------------------------------------------------------------------------------------

            /*
            Matrix<double> xStreakWithPureWildSub_Total = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureWStreakFollowedByNotXW_Total = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> mixedWSubStreakOnly = Matrix<double>.Build.Dense(R, C, 0.0);
            for (int z = 0; z < nWSymbols; z++)
            {
                xStreakWithPureWildSub_Total += XStreakWithPureWildSub(nXInReel, nWInReel, nSInReel, nReel, wild, z);
                pureWStreakFollowedByNotXW_Total += PureWildStreakFollowedByNotXW(nXInReel, nWInReel, nSInReel, nReel, wild, 0);
            }
            mixedWSubStreakOnly = xStreakWithWSub - wStreakFollowedByNotXW - xStreakWithPureWildSub_Total - pureWStreakFollowedByNotXW_Total - xStreakWithoutWSub;
            Matrix<double> payMixedWSubStreakOnly = mixedWSubStreakOnly.PointwiseMultiply(payXMat);
            */
            /*
            Matrix<double> xStreakWithMixedSub = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> payXStreakWithMixedSub = Matrix<double>.Build.Dense(R, C, 0.0);
            xStreakWithMixedSub = xStreakWithWSub - xStreakWithoutWSub;
            for (int z = 0; z < nWSymbols; z++)
            {
                xStreakWithMixedSub -= PureWildStreakFollowedByNotXW(nXInReel, nWInReel, nSInReel, nReel, wild, z);
            }
            payXStreakWithMixedSub = xStreakWithMixedSub.PointwiseMultiply(payXMat);
            */

            // -------------------------------------------------------------------------------------------------------------------
            // Non-Mixed Wilds Substitutions
            // -------------------------------------------------------------------------------------------------------------------
            // if i want to put into function later, here are the inputs: payWMat, payXMat
            int Z = payWMat.RowCount;
            int K = payWMat.ColumnCount - 1;
            // double[,,,] WWZP = new double[Z, K, R, C];
            double[,,] wwzp = new double[K, R, C];
            //double[,,] matStorage1 = new double[K, R, C];
            //double[,,] matStorage2 = new double[K, R, C];
            double[,] wwzp_k = new double[R, C];
            double pureWildPayingMore = 0;
            double pureWouldOtherwisePay = 0;
            Vector<double> sumRows1, sumRows2, col1, col2, col3, prod1, prod2;
            Matrix<double> xFollowsPureWStreak = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureWStreakFollowedByXThenLXW = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureTempMat0 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureTempMat1 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureTempMat2 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureMat1 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureMat2 = Matrix<double>.Build.Dense(R, C, 0.0);
            int N, c0, J, j;
            for (int z = 0; z < Z; z++)
            {
                //Console.WriteLine("z: " + z);
                wwzp = WhereWildZPays(payWMat, payXMat, K + 1, z + 1, pureWildSubMultiplier[z]);
                xFollowsPureWStreak = XFollowsPureWStreak(nXInReel, nWInReel, nSInReel, nReel, wild, z);
                for (int k = 0; k < K; k++)
                {
                    N = 0;
                    J = C - (k + 1);
                    j = 0 + J;
                    wwzp_k = SubArray2D(wwzp, k);
                    pureTempMat0 = Matrix<double>.Build.DenseOfArray(wwzp_k);
                    pureTempMat1 = pureTempMat0 * payWMat[z, k];
                    pureTempMat2 = pureTempMat0.PointwiseMultiply(payXMat) * pureWildSubMultiplier[z];
                    c0 = k + 1;
                    for (int c = c0; c < C; c++)
                    {
                        if (c - 1 <= k)
                        {
                            col1 = pureTempMat1.Column(c);
                            col3 = xFollowsPureWStreak.Column(c);
                            prod1 = col1.PointwiseMultiply(col3);
                            pureTempMat1.SetColumn(c, prod1);

                            col2 = pureTempMat2.Column(c);
                            prod2 = col2.PointwiseMultiply(col3);
                            pureTempMat2.SetColumn(c, prod2);
                        }
                        else
                        {
                            N++;
                            pureWStreakFollowedByXThenLXW = PureWildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, N, z);
                            col1 = pureTempMat1.Column(c);
                            col3 = pureWStreakFollowedByXThenLXW.Column(j);
                            prod1 = col1.PointwiseMultiply(col3);
                            pureTempMat1.SetColumn(c, prod1);

                            col2 = pureTempMat2.Column(c);
                            prod2 = col2.PointwiseMultiply(col3);
                            pureTempMat2.SetColumn(c, prod2);
                            j--;
                        }
                    }
                    pureMat1 = pureMat1 + pureTempMat1;
                    pureMat2 = pureMat2 + pureTempMat2;
                    //Console.WriteLine("k: " + k);
                    sumRows1 = pureTempMat1.RowSums();
                    sumRows2 = pureTempMat2.RowSums();
                    pureWildPayingMore = pureWildPayingMore + sumRows1.Sum();
                    pureWouldOtherwisePay = pureWouldOtherwisePay + sumRows2.Sum();
                    //Console.WriteLine("payTempMat: " + pureTempMat1);
                    //Console.WriteLine("payTempMat2: " + pureTempMat2);
                }
            }
            Console.WriteLine("pureWildPayingMore: " + pureWildPayingMore);
            Console.WriteLine("pureWouldOtherwisePay: " + pureWouldOtherwisePay);


            // -------------------------------------------------------------------------------------------------------------------
            // Mixed Wilds Substitutions
            // -------------------------------------------------------------------------------------------------------------------

            double mixedWildPayingMore = 0;
            double mixedWouldOtherwisePay = 0;
            Matrix<double> xFollowsMixedWStreak = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> wStreakFollowedByXThenLXW = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> tempMat0 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> tempMat1 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> tempMat2 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> mat1 = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> mat2 = Matrix<double>.Build.Dense(R, C, 0.0);


            wwzp = WhereWildZPays(payWMixedMat, payXMat, K + 1, 1, mixedWildSubMultiplier);
            xFollowsMixedWStreak = XFollowsWStreak(nXInReel, nWInReel, nSInReel, nReel, wild);
            for (int z = 0; z < Z; z++)
            {
                xFollowsMixedWStreak = xFollowsMixedWStreak - XFollowsPureWStreak(nXInReel, nWInReel, nSInReel, nReel, wild, z);
            }
            Console.WriteLine("xFollowsMixedWStreak: " + xFollowsMixedWStreak);
            for (int k = 0; k < K; k++)
            {
                N = 0;
                J = C - (k + 1);
                j = 0 + J;
                wwzp_k = SubArray2D(wwzp, k);
                tempMat0 = Matrix<double>.Build.DenseOfArray(wwzp_k);
                tempMat1 = tempMat0 * payWMixedMat[0, k];
                tempMat2 = tempMat0.PointwiseMultiply(payXMat) * mixedWildSubMultiplier;
                c0 = k + 1;
                for (int c = c0; c < C; c++)
                {
                    if (c - 1 <= k)
                    {
                        col1 = tempMat1.Column(c);
                        col3 = xFollowsMixedWStreak.Column(c);
                        prod1 = col1.PointwiseMultiply(col3);
                        tempMat1.SetColumn(c, prod1);

                        col2 = tempMat2.Column(c);
                        prod2 = col2.PointwiseMultiply(col3);
                        tempMat2.SetColumn(c, prod2);
                    }
                    else
                    {
                        N++;
                        wStreakFollowedByXThenLXW = WildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, N);
                        for (int z = 0; z < Z; z++)
                        {
                            wStreakFollowedByXThenLXW = wStreakFollowedByXThenLXW - PureWildStreakFollowedByX_NXW(nXInReel, nWInReel, nSInReel, nReel, wild, N, z);
                        }
                        Console.WriteLine("wStreakFollowedByXThenLXW: " + wStreakFollowedByXThenLXW);
                        col1 = tempMat1.Column(c);
                        col3 = wStreakFollowedByXThenLXW.Column(j);
                        prod1 = col1.PointwiseMultiply(col3);
                        tempMat1.SetColumn(c, prod1);

                        col2 = tempMat2.Column(c);
                        prod2 = col2.PointwiseMultiply(col3);
                        tempMat2.SetColumn(c, prod2);
                        j--;
                    }
                }
                mat1 = mat1 + tempMat1;
                mat2 = mat2 + tempMat2;
                //Console.WriteLine("k: " + k);
                sumRows1 = tempMat1.RowSums();
                sumRows2 = tempMat2.RowSums();
                mixedWildPayingMore = mixedWildPayingMore + sumRows1.Sum();
                mixedWouldOtherwisePay = mixedWouldOtherwisePay + sumRows2.Sum();
                //Console.WriteLine("payTempMat: " + tempMat1);
                //Console.WriteLine("payTempMat2: " + tempMat2);
            }

            Console.WriteLine("mixedWildPayingMore: " + mixedWildPayingMore);
            Console.WriteLine("mixedWouldOtherwisePay: " + mixedWouldOtherwisePay);


            // Pure Wild Pay ------------------
            Matrix<double> payPureWildsMat = Matrix<double>.Build.Dense(nWSymbols, C, 0.0); ;
            payPureWildsMat = wPureTypeStreakWithoutWSub.PointwiseMultiply(payWMat);
            Console.WriteLine("payPureWildsMat: " + payPureWildsMat);
            double PureWildPay = payPureWildsMat.RowSums().Sum() + pureMat1.RowSums().Sum();
            Console.WriteLine("PureWildPay: " + PureWildPay);


            // Mixed Wild Pay -----------------
            Vector<double> wPureTypeStreakColumnSums = wPureTypeStreakWithoutWSub.ColumnSums();
            Vector<double> wMixedOnlyStreakWithoutWSub = wMixedStreakWithoutWSub - wPureTypeStreakColumnSums;
            Vector<double> payWMixedOnlyStreakWithoutWSub = wMixedOnlyStreakWithoutWSub.PointwiseMultiply(payWMixedVec);
            Console.WriteLine("payWMixedOnlyStreakWithoutWSub: " + payWMixedOnlyStreakWithoutWSub);
            Vector<double> MixedWildPayMVec = payWMixedOnlyStreakWithoutWSub;
            double MixedWildPay = MixedWildPayMVec.Sum() + mat1.RowSums().Sum();
            Console.WriteLine("MixedWildPay: " + MixedWildPay);


            // Symbols Pay --------------------
            Matrix<double> xStreakWithPureWildSub_Temp = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureWildStreakFollowedByNotXW_Temp = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> xStreakWithPureWildSub_Total = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> xStreakWithPureWildSub_Total_wMultiplier = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureWildStreakFollowedByNotXW_Total = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pureWildStreakFollowedByNotXW_Total_wMultiplier = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pXStreakWithPureW_Total = Matrix<double>.Build.Dense(R, C, 0.0);
            Matrix<double> pXStreakWithPureW_Total_wMultiplier = Matrix<double>.Build.Dense(R, C, 0.0);
            for (int z = 0; z < nWSymbols; z++)
            {
                xStreakWithPureWildSub_Temp = XStreakWithPureWildSub(nXInReel, nWInReel, nSInReel, nReel, wild, z);
                pureWildStreakFollowedByNotXW_Temp = PureWildStreakFollowedByNotXW(nXInReel, nWInReel, nSInReel, nReel, wild, z);

                xStreakWithPureWildSub_Total += xStreakWithPureWildSub_Temp;
                pureWildStreakFollowedByNotXW_Total += pureWildStreakFollowedByNotXW_Temp;

                xStreakWithPureWildSub_Total_wMultiplier += (xStreakWithPureWildSub_Temp - xStreakWithoutWSub) * pureWildSubMultiplier[z];
                pureWildStreakFollowedByNotXW_Total_wMultiplier += pureWildStreakFollowedByNotXW_Temp * pureWildSubMultiplier[z];
            }
            pXStreakWithPureW_Total = xStreakWithPureWildSub_Total - pureWildStreakFollowedByNotXW_Total;
            pXStreakWithPureW_Total_wMultiplier = xStreakWithPureWildSub_Total_wMultiplier - pureWildStreakFollowedByNotXW_Total_wMultiplier;
            Matrix<double> pXStreakWithW = xStreakWithWSub - wStreakFollowedByNotXW;
            Matrix<double> pXStreakWithMixedWOnly = pXStreakWithW - pXStreakWithPureW_Total;
            Console.WriteLine("pXStreakWithMixedWOnly: " + pXStreakWithMixedWOnly);
            Matrix<double> payXStreakWithMixedWOnly = pXStreakWithMixedWOnly.PointwiseMultiply(payXMat) * mixedWildSubMultiplier;
            Console.WriteLine("payXStreakWithMixedWOnly: " + payXStreakWithMixedWOnly);
            Matrix<double> payXStreakWithPureW_Total = pXStreakWithPureW_Total_wMultiplier.PointwiseMultiply(payXMat);
            Matrix<double> SymbolsPayMat = payXStreakWithPureW_Total + payXStreakWithMixedWOnly - pureMat2 - mat2 + xStreakWithoutWSub.PointwiseMultiply(payXMat) * nWSymbols;
            Vector<double> SymbolsPayVec = SymbolsPayMat.RowSums();
            Console.WriteLine("SymbolsPayVec: " + SymbolsPayVec);


            // Scatter Pay --------------------
            Matrix<double> PScattersMat = Matrix<double>.Build.Dense(nSSymbols, C, 0.0);
            Vector<double> PNScatters;
            for (int nS = 1; nS < C + 1; nS++)
            {
                PNScatters = ProbabilityOfNScatters(nReelVec, nSInReel, nRowsVec, nCols, nS);
                PScattersMat.SetColumn(nS - 1, PNScatters);
            }
            Matrix<double> ScatterPayMat = PScattersMat.PointwiseMultiply(paySMat);
            Console.WriteLine("PScattersMat: " + PScattersMat);
            Console.WriteLine("ScatterPayMat: " + ScatterPayMat);
            double ScatterPay = ScatterPayMat.RowSums().Sum();
            Console.WriteLine("RTP_Scatter: " + ScatterPay);

            // Bonus Pay -----------------------
            double[] BonusPay = EvaluateAllBonusSymbols(reelBSymbols, payBTable, reels, nRows);

            for (int i = 0; i < reelBSymbols.Length; i++)
                Console.WriteLine($"{reelBSymbols[i]} RTP = {BonusPay[i]:F6}");

            double BonusPayTotal = BonusPay.Sum();

            // RTP -----------------------------
            double RTP_BaseGame = (PureWildPay + MixedWildPay + SymbolsPayVec.Sum()) / coinsPerLine;
            //Console.WriteLine("RTP_BaseGame: " + RTP_BaseGame);
            RTP_BaseGame = RTP_BaseGame + ScatterPay + BonusPayTotal;
            Console.WriteLine("RTP_BaseGame: " + RTP_BaseGame);

            // Free Spin RTP Calcs -------------
            double RTP;
            if (freeSpins.Any(x => x))
            {
                double pFreeSpin;
                double[] PFreeSpins = new double[nSSymbols];
                for (int s = 0; s < nSSymbols; s++)
                {
                    if (freeSpins[s])
                    {
                        pFreeSpin = 0;
                        for (int nS = freeSpinTrigger[s]; nS <= nCols; nS++)
                        {
                            pFreeSpin += PScattersMat[s, nS - 1];
                        }
                        PFreeSpins[s] = pFreeSpin;
                    }
                }
                /*Console.WriteLine("PFreeSpins:");
                foreach (double item in PFreeSpins)
                {
                    Console.WriteLine(item);
                }*/
                int nIterations = 10;
                double RTP_FreeSpins = 0;
                double p = 0;
                Vector<double> PFreeSpinsVec = Vector<double>.Build.DenseOfArray(PFreeSpins);
                double[] nFreeSpinsDouble = nFreeSpins.Select(x => (double)x).ToArray();
                Vector<double> nFreeSpinsVec = Vector<double>.Build.DenseOfArray(nFreeSpinsDouble);
                Vector<double> pFreeSpinXnFreeSpin = PFreeSpinsVec.PointwiseMultiply(nFreeSpinsVec);
                for (int s = 0; s < nSSymbols; s++)
                {
                    if (freeSpinRetrigger[s] && freeSpins[s])
                    {
                        for (int i = 1; i <= nIterations; i++)
                        {
                            RTP_FreeSpins += RTP_BaseGame * freeSpinMultiplier[s] * pFreeSpinXnFreeSpin.PointwisePower(i).Sum();
                        }
                    }
                    else
                    {
                        RTP_FreeSpins += RTP_BaseGame * freeSpinMultiplier[s] * PFreeSpins[s] * nFreeSpins[s];
                    }
                }
                RTP = RTP_BaseGame + RTP_FreeSpins;

                Console.WriteLine("RTP_FreeSpins: " + RTP_FreeSpins);
            }
            else
            {
                RTP = RTP_BaseGame;
            }
            Console.WriteLine("RTP: " + RTP);

            Console.ReadLine();
        }
        /* ================================================================================================================== */
        /*                                               Method Definitions                                                   */
        /* ================================================================================================================== */
        static Matrix<double> HowManyOfEachSymbolInReel(string[] reelSymbols, string[][] reels)
        {
            Matrix<double> result = Matrix<double>.Build.Dense(reelSymbols.Length, reels.Length, 0.0);
            int C = reels.Length;
            int R = reelSymbols.Length;
            for (int c = 0; c < C; c++)
            {
                for (int r = 0; r < R; r++)
                {
                    result[r, c] = reels[c].Count(str => str == reelSymbols[r]);
                }
            }
            return result;
        }
        static int[] ReelTotals(string[][] reels)
        {
            int[] result = new int[reels.Length];
            for (int i = 0; i < reels.Length; i++)
            {
                result[i] = reels[i].Length;
            }
            return result;
        }
        static Matrix<double> XStreakWithoutWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    vec2 = nXInReel.Column(i);
                    vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = vec1.PointwiseMultiply(1 - vec2 / nReel[i]);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> XMaxStreakWithoutWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(R, 0.0);
            Vector<double> vec1, vec2;
            int c = C - 1;
            vec1 = Vector<double>.Build.Dense(R, 1.0);
            for (int i = 0; i < c + 1; i++)
            {
                vec2 = nXInReel.Column(i);
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
            }
            result = vec1;
            return result;
        }
        /*
        static Matrix<double> WStreakWithoutWildSub(Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool scatter, bool mixedWild)
        {
            int R = nWInReel.RowCount;
            int C = nWInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            Vector<double> vec0 = Vector<double>.Build.Dense(R, 0.0);
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    if (mixedWild)
                    {
                        vec2 = vec0 + nWInReel.Column(i).Sum();
                    }
                    else
                    {
                        vec2 = vec0 + nWInReel.Column(i);
                    }
                    vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    if (scatter & mixedWild)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nSInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else if (!scatter & mixedWild)
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                    else if (scatter & !mixedWild)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nSInReel.Column(i).Sum() + nWInReel.Column(i).Sum() - nWInReel.Column(i)) / nReel[i]);
                    }
                    else // (!scatter & !mixedWild)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nWInReel.Column(i).Sum() - nWInReel.Column(i)) / nReel[i]);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }*/
        static Matrix<double> WPureTypeStreakWithoutWildSub(Matrix<double> nWInReel, Matrix<double> nSInReel, Matrix<double> nBInReel, int[] nReel, bool scatter, bool bonus)
        {
            int R = nWInReel.RowCount;
            int C = nWInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            Vector<double> vec0 = Vector<double>.Build.Dense(R, 0.0);
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    vec2 = vec0 + nWInReel.Column(i);
                    vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    if (scatter && !bonus)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nSInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else if (bonus && !scatter)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nBInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else if (scatter && bonus)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nSInReel.Column(i).Sum() + nBInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else // (!scatter && !bonus)
                    {
                        vec1 = Vector<double>.Build.Dense(R, 0.0);
                    }

                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> WMixedStreakWithoutWildSub(Matrix<double> nWInReel, Matrix<double> nSInReel, Matrix<double> nBInReel, int[] nReel, bool scatter, bool bonus)
        {
            int R = nWInReel.RowCount;
            int C = nWInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(C, 0.0);
            double scal1, scal2;
            for (int c = 0; c < C; c++)
            {
                scal1 = 1;
                for (int i = 0; i < c + 1; i++)
                {
                    scal2 = nWInReel.Column(i).Sum();
                    scal1 = scal1 * scal2 / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    if (scatter && bonus)
                    {
                        scal1 = scal1 * (nSInReel.Column(i).Sum() + nBInReel.Column(i).Sum()) / nReel[i];
                    }
                    else if (scatter && !bonus)
                    {
                        scal1 = scal1 * (nSInReel.Column(i).Sum()) / nReel[i];
                    }
                    else if (bonus && !scatter)
                    {
                        scal1 = scal1 * (nBInReel.Column(i).Sum()) / nReel[i];
                    }
                    else // (!scatter)
                    {
                        scal1 = 0;
                    }
                }
                result[c] = scal1;
            }
            return result;
        }
        static Matrix<double> XStreakWithWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    vec2 = nXInReel.Column(i) + nWInReel.Column(i).Sum();
                    vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = vec1.PointwiseMultiply(1 - vec2 / nReel[i]);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> XMaxStreakWithWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(R, 0.0);
            Vector<double> vec1, vec2;
            int c = C - 1;
            vec1 = Vector<double>.Build.Dense(R, 1.0);
            for (int i = 0; i < c + 1; i++)
            {
                vec2 = nXInReel.Column(i) + nWInReel.Column(i).Sum();
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
            }
            result = vec1;
            return result;
        }
        static Matrix<double> XStreakWithPureWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    vec2 = nXInReel.Column(i) + nWInReel[Wi, i];
                    vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = vec1.PointwiseMultiply(1 - vec2 / nReel[i]);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> XMaxStreakWithPureWildSub(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(R, 0.0);
            Vector<double> vec1, vec2;
            int c = C - 1;
            vec1 = Vector<double>.Build.Dense(R, 1.0);
            for (int i = 0; i < c + 1; i++)
            {
                vec2 = nXInReel.Column(i) + nWInReel[Wi, i];
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[i];
            }
            result = vec1;
            return result;
        }
        static Matrix<double> XFollowsWStreak(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            for (int c = 1; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c; i++)
                {
                    scal1 = nWInReel.Column(i).Sum();
                    vec1 = scal1 / nReel[i] * vec1;
                }
                vec2 = nXInReel.Column(c);
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[c];
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Matrix<double> XFollowsPureWStreak(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            for (int c = 1; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c; i++)
                {
                    scal1 = nWInReel[Wi, i];
                    vec1 = scal1 / nReel[i] * vec1;
                }
                vec2 = nXInReel.Column(c);
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[c];
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        /*
        static Matrix<double> WildStreakFollowedByY(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            for (int c = 0; c < C-1; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c+1; i++)
                {
                    scal1 = nWInReel.Column(i).Sum();
                    vec1 = scal1 / nReel[i] * vec1;
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        */
        static Matrix<double> WildStreakFollowedByNotXW(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    scal1 = nWInReel.Column(i).Sum();
                    vec1 = scal1 / nReel[i] * vec1;
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> MixedMaxWildStreak(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(R, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            int c = C - 1;
            vec1 = Vector<double>.Build.Dense(R, 1.0);
            for (int i = 0; i < c + 1; i++)
            {
                scal1 = nWInReel.Column(i).Sum();
                vec1 = scal1 / nReel[i] * vec1;
            }
            result = vec1;
            return result;
        }
        static Matrix<double> PureWildStreakFollowedByNotXW(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            for (int c = 0; c < C; c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    scal1 = nWInReel[Wi, i];
                    vec1 = scal1 / nReel[i] * vec1;
                }
                if (c < C - 1)
                {
                    int i = c + 1;
                    vec2 = nXInReel.Column(i);
                    if (wild)
                    {
                        vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else
                    {
                        vec1 = Vector<double>.Build.Dense(R);
                    }
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> PureMaxWildStreak(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Vector<double> result = Vector<double>.Build.Dense(R, 0.0);
            Vector<double> vec1, vec2;
            double scal1;
            int c = C - 1;
            vec1 = Vector<double>.Build.Dense(R, 1.0);
            for (int i = 0; i < c + 1; i++)
            {
                scal1 = nWInReel[Wi, i];
                vec1 = scal1 / nReel[i] * vec1;
            }
            result = vec1;
            return result;
        }
        static Matrix<double> WildStreakFollowedByX_NXW(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int N)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;

            for (int c = 0; c < C - (N + 1); c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    scal1 = nWInReel.Column(i).Sum();
                    vec1 = scal1 / nReel[i] * vec1;
                }
                int j = c + 1;
                vec2 = nXInReel.Column(j);
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[j];
                for (int i = 0; i < N; i++)
                {
                    j++;
                    vec2 = nXInReel.Column(j);
                    vec1 = vec1.PointwiseMultiply((vec2 + nWInReel.Column(j).Sum()) / nReel[j]);
                }
                j++;
                if (j < C)
                {
                    vec2 = nXInReel.Column(j);
                    vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(j).Sum()) / nReel[j]);
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Matrix<double> PureWildStreakFollowedByX_NXW(Matrix<double> nXInReel, Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool wild, int N, int Wi)
        {
            int R = nXInReel.RowCount;
            int C = nXInReel.ColumnCount;
            Matrix<double> result = Matrix<double>.Build.Dense(R, C, 0.0);
            Vector<double> vec1, vec2;
            double scal1;

            for (int c = 0; c < C - (N + 1); c++)
            {
                vec1 = Vector<double>.Build.Dense(R, 1.0);
                for (int i = 0; i < c + 1; i++)
                {
                    scal1 = nWInReel[Wi, i];
                    vec1 = scal1 / nReel[i] * vec1;
                }
                int j = c + 1;
                vec2 = nXInReel.Column(j);
                vec1 = vec1.PointwiseMultiply(vec2) / nReel[j];
                for (int i = 0; i < N; i++)
                {
                    j++;
                    vec2 = nXInReel.Column(j);
                    vec1 = vec1.PointwiseMultiply((vec2 + nWInReel.Column(j).Sum()) / nReel[j]);
                }
                j++;
                if (j < C)
                {
                    vec2 = nXInReel.Column(j);
                    vec1 = vec1.PointwiseMultiply(1 - (vec2 + nWInReel.Column(j).Sum()) / nReel[j]);
                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static double[,,] WhereWildZPays(Matrix<double> payWMat, Matrix<double> payXMat, int nCols, int Z, double wMultiplier)
        {
            int R = payXMat.RowCount;
            int C = payXMat.ColumnCount;
            double[,,] result = new double[C - 1, R, C];
            Array.Clear(result, 0, result.Length);
            for (int k = 0; k < C - 1; k++)
            {
                for (int i = 0; i < R; i++)
                {
                    for (int j = k + 1; j < C; j++)
                    {
                        if (payWMat[Z - 1, k] >= payXMat[i, j] * wMultiplier)
                        {
                            result[k, i, j] = 1;
                        }
                    }
                }
            }
            return result;
        }
        static double[,] ScatterCombinationsGenerator(int nCols, int nS)
        {
            int nCombinations = (int)(SpecialFunctions.Factorial(nCols) / (int)SpecialFunctions.Factorial(nCols - nS)) / (int)SpecialFunctions.Factorial(nS);
            double[,] table = new double[nCombinations, nCols];
            Array.Clear(table, 0, table.Length);
            int z = 0;
            for (int i = 0; i < nCols + 1 - nS; i++)
            {
                table[z, i] = 1;
                if (nS > 1)
                {
                    for (int j = i + 1; j < nCols + 2 - nS; j++)
                    {
                        table[z, i] = 1;
                        table[z, j] = 1;
                        if (nS > 2)
                        {
                            for (int k = j + 1; k < nCols + 3 - nS; k++)
                            {
                                table[z, i] = 1;
                                table[z, j] = 1;
                                table[z, k] = 1;
                                if (nS > 3)
                                {
                                    for (int l = k + 1; l < nCols + 4 - nS; l++)
                                    {
                                        table[z, i] = 1;
                                        table[z, j] = 1;
                                        table[z, k] = 1;
                                        table[z, l] = 1;
                                        if (nS > 4)
                                        {
                                            for (int m = l + 1; m < nCols + 5 - nS; m++)
                                            {
                                                table[z, i] = 1;
                                                table[z, j] = 1;
                                                table[z, k] = 1;
                                                table[z, l] = 1;
                                                table[z, m] = 1;
                                                if (nS > 5)
                                                {
                                                    for (int n = m + 1; n < nCols + 6 - nS; n++)
                                                    {
                                                        table[z, i] = 1;
                                                        table[z, j] = 1;
                                                        table[z, k] = 1;
                                                        table[z, l] = 1;
                                                        table[z, m] = 1;
                                                        table[z, n] = 1;
                                                        if (nS > 6)
                                                        {
                                                            for (int o = n + 1; o < nCols + 7 - nS; o++)
                                                            {
                                                                table[z, i] = 1;
                                                                table[z, j] = 1;
                                                                table[z, k] = 1;
                                                                table[z, l] = 1;
                                                                table[z, m] = 1;
                                                                table[z, n] = 1;
                                                                table[z, o] = 1;
                                                                z = z + 1;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            z++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    z++;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            z++;
                                        }
                                    }
                                }
                                else
                                {
                                    z++;
                                }
                            }
                        }
                        else
                        {
                            z++;
                        }
                    }
                }
                else
                {
                    z++;
                }
            }
            return table;
        }
        static Vector<double> ProbabilityOfNScatters(Vector<double> nReelVec, Matrix<double> nSInReel, Vector<double> nRowsVec, int nCols, int nS)
        {
            int R = nSInReel.RowCount;
            //double[] result = new double[R];
            Vector<double> vec, mFactor;
            Matrix<double> pScatter = nSInReel.Clone();

            for (int i = 0; i < R; i++)
            {
                vec = nSInReel.Row(i).PointwiseDivide(nReelVec);
                vec = vec.PointwiseMultiply(nRowsVec);
                pScatter.SetRow(i, vec);
            }
            double[,] scatterCombinations = ScatterCombinationsGenerator(nCols, nS);
            int r = scatterCombinations.GetLength(0);
            int c = scatterCombinations.GetLength(1);

            Matrix<double> p = Matrix<double>.Build.Dense(R, r, 1.0);
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (scatterCombinations[i, j] == 1)
                    {
                        mFactor = pScatter.Column(j);
                    }
                    else
                    {
                        mFactor = 1 - pScatter.Column(j);
                    }

                    p.SetColumn(i, p.Column(i).PointwiseMultiply(mFactor));
                }
            }
            Vector<double> PNScatter = p.RowSums();
            //Console.WriteLine(PNScatter);
            return PNScatter;
        }
        static double[,] SubArray2D(double[,,] Array3D, int k)
        {
            int numRows = Array3D.GetLength(1);
            int numCols = Array3D.GetLength(2);
            double[,] result = new double[numRows, numCols];
            for (int i = Array3D.GetLowerBound(1); i <= Array3D.GetUpperBound(1); i++)
            {
                for (int j = Array3D.GetLowerBound(2); j <= Array3D.GetUpperBound(2); j++)
                {
                    result[i, j] = Array3D[k, i, j];
                }
            }
            return result;
        }
        static double[,,] StoreMatrixTo3DArray(Matrix<double> mat, double[,,] matStorage, int k)
        {
            int rows = mat.RowCount;
            int cols = mat.ColumnCount;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matStorage[k, i, j] = mat[i, j];
                }
            }
            return matStorage;
        }
        static string[][] ExpandColumns(string[][] reels, int[] extra)
        {
            int rows = reels.Length;

            // Use LINQ to create the new jagged array
            string[][] result = Enumerable.Range(0, rows)
                .Select(row => Enumerable.Range(0, reels[row].Length + extra[row] - 1)
                    .Select(col => reels[row][col % reels[row].Length])
                    .ToArray())
                .ToArray();

            return result;
        }
        static int[][] OnScreenBonusCount(string[][] virtualReels, int[] nRows, string reelBSymbol)
        {
            int nReels = virtualReels.Length;
            int[][] result = new int[nReels][];

            for (int c = 0; c < nReels; c++)
            {
                // original strip length before you padded it with (nRows[c]-1) symbols
                int originalLen = virtualReels[c].Length - nRows[c] + 1;
                result[c] = new int[originalLen];

                for (int start = 0; start < originalLen; start++)
                {
                    int count = 0;
                    for (int offset = 0; offset < nRows[c]; offset++)
                    {
                        if (virtualReels[c][start + offset] == reelBSymbol)
                            count++;
                    }
                    result[c][start] = count;
                }
            }
            return result;
        }
        static int[][] BuildBonusRangePerReel(int[] nRows)
        {
            if (nRows == null)
                throw new ArgumentNullException(nameof(nRows));

            int[][] result = new int[nRows.Length][];

            for (int i = 0; i < nRows.Length; i++)
            {
                int max = nRows[i];               // maximum bonus symbols on reel i
                result[i] = new int[max + 1];     // include 0, so length = max+1

                for (int k = 0; k <= max; k++)
                    result[i][k] = k;             // {0,1,2,…,max}
            }

            return result;
        }
        static int[][] HistogramBonusCounts(int[][] bonusCounts, int[] nRows)
        {
            if (bonusCounts == null) throw new ArgumentNullException(nameof(bonusCounts));
            if (nRows == null) throw new ArgumentNullException(nameof(nRows));
            if (bonusCounts.Length != nRows.Length)
                throw new ArgumentException("bonusCounts and nRows must have same length.");

            int reels = bonusCounts.Length;
            int[][] result = new int[reels][];

            for (int r = 0; r < reels; r++)
            {
                int max = nRows[r];                 // maximum possible bonus symbols on reel r
                int[] hist = new int[max + 1];      // 0..max
                foreach (int val in bonusCounts[r])
                {
                    if (val < 0 || val > max)
                        throw new InvalidOperationException($"Unexpected count {val} on reel {r}");
                    hist[val]++;                    // tally
                }
                result[r] = hist;
            }

            return result;
        }
        static int[,] CartesianProduct(int[][] ranges)
        {
            if (ranges == null || ranges.Length == 0)
                return new int[0, 0];

            int reels = ranges.Length;
            long rowsLong = 1;
            foreach (var col in ranges)
                rowsLong *= col.Length;

            if (rowsLong > int.MaxValue)
                throw new OverflowException("Too many combinations for a single int[,]");

            int rows = (int)rowsLong;
            int[,] result = new int[rows, reels];

            // iterative mixed-radix counter
            int[] counters = new int[reels];

            for (int row = 0; row < rows; row++)
            {
                for (int r = 0; r < reels; r++)
                    result[row, r] = ranges[r][counters[r]];

                // increment counters (right-most digit first)
                for (int r = reels - 1; r >= 0; r--)
                {
                    counters[r]++;
                    if (counters[r] < ranges[r].Length) break;
                    counters[r] = 0; // carry
                }
            }
            return result;
        }

        static int[,] MapCombosToHistogram(int[][] bonusHistogram, int[,] combos)
        {
            int rows = combos.GetLength(0);
            int reels = combos.GetLength(1);

            int[,] result = new int[rows, reels];

            for (int i = 0; i < rows; i++)
                for (int r = 0; r < reels; r++)
                {
                    int bonusCount = combos[i, r];
                    result[i, r] = bonusHistogram[r][bonusCount];   // axis order: reel,bonus
                }

            return result;
        }

        static int[,] MaskCombosByHistogram(int[][] bonusHistogram, int[,] combos)
        {
            int rows = combos.GetLength(0);
            int reels = combos.GetLength(1);

            int[,] masked = new int[rows, reels];

            for (int i = 0; i < rows; i++)
                for (int r = 0; r < reels; r++)
                {
                    int bonusCount = combos[i, r];
                    int histValue = bonusHistogram[r][bonusCount];

                    masked[i, r] = (histValue == 0) ? 0 : bonusCount;
                }

            return masked;
        }
        public static int[] RowSums(int[,] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            int[] result = new int[rows];

            for (int i = 0; i < rows; i++)
            {
                int sum = 0;
                for (int j = 0; j < cols; j++)
                    sum += source[i, j];
                result[i] = sum;
            }
            return result;
        }
        public static int[] RowProducts(int[,] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            int[] result = new int[rows];

            for (int i = 0; i < rows; i++)
            {
                int prod = 1;
                for (int j = 0; j < cols; j++)
                    prod *= source[i, j];
                result[i] = prod;
            }
            return result;
        }
        public static int Product(int[] source)
        {
            int prod = 1;
            for (int i = 0; i < source.Length; i++)
            {
                prod *= source[i];
            }
            return prod;
        }
        public static int[,] GroupAndSumToArray(int[] rowKeys, int[] values)
        {
            if (rowKeys.Length != values.Length)
                throw new ArgumentException("rowKeys and values must be the same length.");

            Dictionary<int, int> grouped = new Dictionary<int, int>();

            for (int i = 0; i < rowKeys.Length; i++)
            {
                int key = rowKeys[i];
                int val = values[i];

                if (!grouped.ContainsKey(key))
                    grouped[key] = 0;

                grouped[key] += val;
            }

            // Convert to int[,]
            int rows = grouped.Count;
            int[,] result = new int[rows, 2];

            int index = 0;
            foreach (var kvp in grouped.OrderBy(k => k.Key))
            {
                result[index, 0] = kvp.Key;   // unique row sum
                result[index, 1] = kvp.Value; // total value
                index++;
            }

            return result;
        }

        static double[] ScaleAndPay(int[,] uniqueSumGrid, int cycleTotal, double[] payVector)
        {
            if (cycleTotal == 0) throw new DivideByZeroException(nameof(cycleTotal));

            int rows = uniqueSumGrid.GetLength(0);
            int cols = payVector.Length;

            // truncate from the END if payout vector is longer
            int len = Math.Min(rows, cols);
            double[] result = new double[len];

            for (int i = 0; i < len; i++)
            {
                double prob = uniqueSumGrid[i, 1] / (double)cycleTotal;
                result[i] = prob * payVector[i];
            }
            return result;
        }

        static double[] EvaluateAllBonusSymbols(
        string[] bonusSymbols,
        double[,] payBTable,
        string[][] reels,
        int[] nRows)
        {
            string[][] virtualReels = ExpandColumns(reels, nRows);
            int[][] bonusRanges = BuildBonusRangePerReel(nRows);
            int[,] combos = CartesianProduct(bonusRanges);
            int nBonusTypes = bonusSymbols.Length;
            double[] rtpPerBonus = new double[nBonusTypes];

            for (int b = 0; b < nBonusTypes; b++)
            {
                string bonusSym = bonusSymbols[b];

                // -------- single-symbol pass --------
                int[][] bonusPerStop = OnScreenBonusCount(virtualReels, nRows, bonusSym);
                int[][] bonusHistograms = HistogramBonusCounts(bonusPerStop, nRows);
                int[,] mapped = MapCombosToHistogram(bonusHistograms, combos);
                int[,] maskedCombos = MaskCombosByHistogram(bonusHistograms, combos);
                int[] rowTotals = RowSums(maskedCombos);
                int[] rowProducts = RowProducts(mapped);
                int[,] uniqueSumGrid = GroupAndSumToArray(rowTotals, rowProducts);

                int cycleTotal = Product(reels.Select(r => r.Length).ToArray());

                // --- grab the payout row for this symbol and compute RTP
                int payCols = payBTable.GetLength(1);
                double[] payVector = new double[payCols];
                for (int c = 0; c < payCols; c++)
                    payVector[c] = payBTable[b, c];

                double[] rtps = ScaleAndPay(uniqueSumGrid, cycleTotal, payVector);
                rtpPerBonus[b] = rtps.Sum();
            }

            return rtpPerBonus;   // one RTP value per bonus type
        }
    }
}