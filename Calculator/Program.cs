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
            int nPayLines = 40;
            double coinsPerLine = 0.5;

            bool[] freeSpins = { false };
            int[] freeSpinTrigger = { 3 };
            bool scatter = true;
            int[] nFreeSpins = { 0 };
            bool[] freeSpinRetrigger = { false };
            bool wild = true;
            int[] freeSpinMultiplier = { 0 };
            double[] pureWildSubMultiplier = { 1 };
            double mixedWildSubMultiplier = 1;

            string[] reelXSymbols = { "High1", "High2", "High3", "High4", "Low1", "Low2", "Low3", "Low4" };
            string[] reelWSymbols = { "Wild1" };
            string[] reelSSymbols = { "Scatter1" };

            // gets transposed. defined this way because of length
            double[,] payXTable = { { 0, 0, 0, 0, 0, 0, 0, 0 },
                                    { 0, 0, 0, 0, 0, 0, 0, 0 },
                                    { 20, 20, 15, 10, 2, 2, 2, 2 },
                                    { 60, 45, 25, 20, 15, 10, 5, 5 },
                                    { 125, 90, 60, 50, 40, 30, 20, 20 } };

            // do not transpose
            double[,] payWTable = { { 0, 0, 30, 150, 2500 } };

            // make this one a vector
            double[] payWMixedTable = { 0, 0, 60, 300, 5000 };

            // do not transpose
            double[,] paySTable = { { 0, 0, 2, 50, 500 } };

            string[] reel1 = { "Wild1", "Wild1", "Wild1", "Low3", "Low3", "High1", "Low4", "Low4", "High2", "Low3", "Low3", "Scatter1", "Low4", "Low4", "High3", "Low1", "Low1", "Low2", "Low2", "High4", "Low3", "Low3", "Low4", "Low4", "Low4", "High1", "Low3", "High1", "Low4", "Low4", "Low4", "Low1", "Low1", "Low2", "Low2", "High3", "Low3", "Low3", "High2", "Low4", "High2", "Low3", "Low3", "Low3", "High4", "Low4", "Low4", "Low1", "Low1", "Low2", "Low2", "Low3", "High3", "High4", "High4" };
            string[] reel2 = { "Wild1", "Wild1", "Wild1", "High1", "Low1", "Low1", "High2", "Low2", "Low2", "High1", "Low1", "Scatter1", "Low2", "Low2", "High2", "Low3", "Low3", "Low3", "Low4", "Low4", "Low4", "High3", "Low1", "Low1", "High4", "High2", "Low2", "High1", "Low1", "High3", "Low2", "Low2", "High4", "Low3", "Low3", "Low3", "Low4", "Low4", "Low4", "Low1", "Low3", "Low4", "High3", "High4", "Low2" };
            string[] reel3 = { "Wild1", "Wild1", "Wild1", "High1", "Low4", "High3", "Low2", "High2", "Low3", "High4", "Low1", "Scatter1", "Low1", "High4", "Low3", "High2", "Low2", "High3", "Low4", "High1", "Scatter", "High2", "Low2", "High4", "Low4", "High1", "Low1", "High3", "Low3", "High1", "High2", "High3", "High4" };
            string[] reel4 = { "Wild1", "Wild1", "Wild1", "High3", "High3", "High4", "High4", "Scatter1", "High3", "High4", "High4", "High1", "High1", "High2", "High2", "Scatter1", "High3", "High3", "High4", "High4", "High1", "High2", "Low1", "Low3", "Low2", "Low4", "Scatter", "Low4", "Low3", "Low2", "Low1", "Low1", "Low2", "Low3", "Low4", "High3" };
            string[] reel5 = { "Wild1", "Wild1", "Wild1", "High1", "High1", "High2", "High2", "Scatter1", "High1", "High2", "High2", "High3", "High4", "High3", "High4", "Scatter1", "Low1", "High3", "Low2", "Low4", "High4", "Low3", "Low2", "Low1", "Scatter1", "High1", "Low4", "High3", "High4", "Low3", "Low1", "Low2", "Low3", "Low4" };

            string[][] reels = { reel1, reel2, reel3, reel4, reel5 };
            int[] nReel = ReelTotals(reels);

            /* ================================================================================================================== */
            /*                                                Main Calculations                                                   */
            /* ================================================================================================================== */

            Vector<double> nRowsVec = Vector<double>.Build.DenseOfArray(nRows.Select(x => (double)x).ToArray());
            double betCost = nPayLines * coinsPerLine;
            int nXSymbols = reelXSymbols.Length;
            int nWSymbols = reelWSymbols.Length;
            int nSSymbols = reelSSymbols.Length;
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
            Console.WriteLine("nWInReel: " + nWInReel);
            Console.WriteLine("nXInReel: " + nXInReel);
            Console.WriteLine("nSInReel: " + nSInReel);

            Matrix<double> xStreakWithoutWSub = XStreakWithoutWildSub(nXInReel, nWInReel, nSInReel, nReel, wild);
            // Matrix<double> wStreakWithoutWSub = WStreakWithoutWildSub(nWInReel, nSInReel, nReel, scatter, mixedWild);
            Matrix<double> wPureTypeStreakWithoutWSub = WPureTypeStreakWithoutWildSub(nWInReel, nSInReel, nReel, scatter);
            Vector<double> wMixedStreakWithoutWSub = WMixedStreakWithoutWildSub(nWInReel, nSInReel, nReel, scatter);
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


            // RTP -----------------------------
            double RTP_BaseGame = (PureWildPay + MixedWildPay + SymbolsPayVec.Sum()) / coinsPerLine;
            //Console.WriteLine("RTP_BaseGame: " + RTP_BaseGame);
            RTP_BaseGame = RTP_BaseGame + ScatterPay;
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
        static Matrix<double> WPureTypeStreakWithoutWildSub(Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool scatter)
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
                    if (scatter)
                    {
                        vec1 = vec1.PointwiseMultiply((vec0 + nSInReel.Column(i).Sum()) / nReel[i]);
                    }
                    else // (!scatter)
                    {
                        vec1 = Vector<double>.Build.Dense(R, 0.0);
                    }

                }
                result.SetColumn(c, vec1);
            }
            return result;
        }
        static Vector<double> WMixedStreakWithoutWildSub(Matrix<double> nWInReel, Matrix<double> nSInReel, int[] nReel, bool scatter)
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
                    if (scatter)
                    {
                        scal1 = scal1 * (nSInReel.Column(i).Sum() / nReel[i]);
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
    }
}