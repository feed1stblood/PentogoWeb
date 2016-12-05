using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PentagoWeb.Model.Board;
using System.Diagnostics;
using System.Windows;

namespace TableMaker
{
    public class Program
    {
        Dictionary<Status, Point> posDic = new Dictionary<Status,Point>(36);
        static int lineNo = 0;
        static List<int>[,] posList = new List<int>[6, 6];
        static List<int>[] rotList = new List<int>[4];

        public void Main(string[] args)
        {
            int n = 5;
            PentagoBoard board = new PentagoBoard();
            for (int i = 0; i < board.TotalWidth; i++)
                for (int j = 0; j < board.TotalHeight; j++)
                {
                    posList[i, j] = new List<int>();
                    posDic.Add(board[i, j], new Point(i, j));
                }
            for (int i = 0; i < 4; i++)
                rotList[i] = new List<int>();
            Process(board, 4, new int[4]);
            OutputPosList();
            OutputRotList();
        }

        void Process(PentagoBoard board, int depth, int[] paras)
        {
            if (depth == 0)
            {
                int n = 5; // # of positions each line
                Point[] line = new Point[n];
                int[] x = new int[n];
                int[] y = new int[n];
                int[] ox = new int[n];
                int[] oy = new int[n];
                for (int i = 0; i < board.TotalWidth; i++)
                    for (int j = 0; j < board.TotalHeight; j++)
                    {
                        Func<bool>[] criteria = { () => i + n <= board.TotalWidth 
                                                 , () => j + n <= board.TotalWidth
                                                 , () => i + n <= board.TotalWidth && j + n <= board.TotalWidth
                                                 , () => i + n <= board.TotalWidth && j - n >= -1};
                        Func<int, int>[] fx = { (c) => c + i 
                                              , (c) => i 
                                              , (c) => c + i 
                                              , (c) => c + i };
                        Func<int, int>[] fy = { (c) => j 
                                              , (c) => c + j
                                              , (c) => c + j 
                                              , (c) => -c + j };

                        for (int k = 0; k < criteria.Length; k++)
                        {
                            if (criteria[k]())
                            {
                                bool[] sectionInvolved = new bool[paras.Length];                               
                                for(int l = 0;l<n;l++)
                                {
                                    x[l] = fx[k](l);
                                    y[l] = fy[k](l);
                                    var status = board[x[l], y[l]];
                                    Point originalPos;
                                    this.posDic.TryGetValue(status,out originalPos);
                                    ox[l] = (int)originalPos.X;
                                    oy[l] = (int)originalPos.Y;
                                    sectionInvolved[this.SectionInvolvedWith(originalPos)] = true;
                                }
                                OutputItem(ox, oy, sectionInvolved, paras);
                            }
                        }

                        #region original implementation
                        //if (i + n <= board.TotalWidth)
                        //{
                        //    int k = 0;
                        //    while (k < n)
                        //    {
                                
                        //        k++;
                        //    }
                        //}

                        ////check for vertical case
                        //if (j + n <= board.TotalWidth)
                        //{
                        //    int k = 0;
                        //    while (k < n)
                        //        k++;
                        //}

                        ////check for diagonal case
                        //if (i + n <= board.TotalWidth && j + n <= board.TotalWidth)
                        //{
                        //    int k = 0;
                        //    while (k < n)
                        //        k++;
                        //}
                        //if (i + n <= board.TotalWidth && j - n >= -1)
                        //{
                        //    int k = 0;
                        //    while (k < n)
                        //        k++;
                        //}
                        #endregion
                    }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    paras[paras.Length - depth] = i;
                    Process(board, depth - 1, paras);
                    board.Sections[paras.Length - depth].StartRotatingClockwise();
                }
            }
        }

        public static void OutputItem(int[] x, int[] y, bool[] sec, int[] secC)
        {
            bool duplicated = false;
            int rotationCount = 0;
            for (int i = 0; i < sec.Length; i++)
            {
                
                if (!sec[i] && secC[i]>0)
                {
                    duplicated = true;
                    break;
                }

                if (sec[i])
                    rotationCount += Math.Abs(GetRCCount(secC[i]));

            }
            if (duplicated)
                return;
            //if (rotationCount > 5)
            //    return;

            string result = "";
            
            for (int i = 0; i < x.Length; i++)
            {
                result += "{" + x[i] + "," + y[i] + "},";
                posList[x[i], y[i]].Add(lineNo);
            }
            result += "{";
            for (int i = 0; i < sec.Length; i++)
            {
                if(sec[i])
                {
                    int trueCount = GetRCCount(secC[i]);
                    result += trueCount + ",";
                    rotList[i].Add(lineNo);
                }
                else
                {
                    //indicates not involved
                    result += "null,";
                }
            }
            result += rotationCount + "},";
            Debug.WriteLine(result);
            lineNo++;
        }

        void OutputPosList()
        {
             for (int i = 0; i < 6; i++)
                 for (int j = 0; j < 6; j++)
                 {
                     string s = i+","+j+" {";
                     posList[i,j].ForEach(e => s += e + ",");
                     s = s.Substring(0, s.Length - 1) + "},";
                     Debug.WriteLine(s);
                 }
        }

        void OutputRotList()
        {
            for (int i = 0; i < 4; i++)
            {
                string s = i + " {";
                rotList[i].ForEach(e => s += e + ",");
                s = s.Substring(0, s.Length - 1) + "},";
                Debug.WriteLine(s);
            }
        }
        /// <summary>
        /// get the number of clockwise rotation, value belongs to {-1,0,1,2}
        /// </summary>
        /// <param name="n">value belongs to {0,1,2,3}</param>
        /// <returns>adjusted value belongs to {-1,0,1,2}</returns>
        public static int GetRCCount(int n)
        {
            if (n == 3)
                return -1;
            return n;
        }

        public int SectionInvolvedWith(Point position)
        {
            int result = 0;
            if (position.X >= 3)
                result ++;
            if (position.Y >= 3)
                result += 2;
            return result;
        }
    }
}
