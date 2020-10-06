// MIT License
//
// Copyright (c) 2020 Sebastian Kotstein
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using skotstein.net.http.reachabilityanalyzer.bulk.model;
using skotstein.net.http.urimodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skotstein.net.http.reachabilityanalyzer.bulk.tools
{
    public class ConsoleOut
    {
        private int _bulkPrintCounter = 0;
        private int _tableWidth = 0;

        private IDictionary<string, int> _lengthOfFields = new Dictionary<string, int>()
            {
                {AnalysisResult.PARAMETER_COUNTER, 8},
                {AnalysisResult.PARAMETER_API_KEY, 8 },
                {AnalysisResult.PARAMETER_API_NAME, 40},
                {AnalysisResult.PARAMETER_API_VERSION_KEY,8},
                {AnalysisResult.PARAMETER_API_VERSION_NAME,14},
                {AnalysisResult.PARAMETER_NUM_ROOTS,6 },
                {AnalysisResult.PARAMETER_NUM_PATH_SEGMENTS,10},
                {AnalysisResult.PARAMETER_NUM_VARIABLE_PATH_SEGMENTS,10},
                {AnalysisResult.PARAMETER_AVG_LINKS_PER_PARAMETER,10 },
                {AnalysisResult.PARAMETER_SHARE_REACHABLE_RESOURCES,15 }
            };

        private IDictionary<string, string> _headerName = new Dictionary<string, string>()
        {
                {AnalysisResult.PARAMETER_COUNTER, "COUNTER"},
                {AnalysisResult.PARAMETER_API_KEY, "API KEY" },
                {AnalysisResult.PARAMETER_API_NAME, "API NAME" },
                {AnalysisResult.PARAMETER_API_VERSION_KEY, "VER. KEY"},
                {AnalysisResult.PARAMETER_API_VERSION_NAME,"VER. NAME"},
                {AnalysisResult.PARAMETER_NUM_ROOTS, "ROOTS"},
                {AnalysisResult.PARAMETER_NUM_PATH_SEGMENTS,"PATH SEGM."},
                {AnalysisResult.PARAMETER_NUM_VARIABLE_PATH_SEGMENTS,"VAR. SEGM."},
                {AnalysisResult.PARAMETER_AVG_LINKS_PER_PARAMETER,"AVG LINKS"},
                {AnalysisResult.PARAMETER_SHARE_REACHABLE_RESOURCES,"REACHABILITY"}
        };

        public ConsoleOut()
        {
            _tableWidth = 1;
            foreach(KeyValuePair<string,int> lengthOfFields in _lengthOfFields)
            {
                _tableWidth += lengthOfFields.Value + 3;
            }
        }

        public void PrintLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public void PrintLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }


        #region Single Analysis

        public void PrintUriModel(UriModel uriModel)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(uriModel.ToString());
            Console.WriteLine();
        }

        public void PrintReachabilityPaths(IList<PathSegment> pathSegments)
        {
            Console.ForegroundColor = ConsoleColor.White;
            foreach(PathSegment pathSegment in pathSegments)
            {
                Console.WriteLine(pathSegment.UriPath);
                foreach(ReachabilityPath reachabilityPath in ((CustomizedPathSegment)pathSegment).ReachabilityPaths)
                {
                    Console.WriteLine(reachabilityPath.ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        #endregion

        #region Bulk Analysis



        public void PrintHeaderBulkAnalysisStart(string startDate)
        {
            _bulkPrintCounter = 0;
            PrintLine("Start @ " + startDate);
            PrintTableLine();
            string line = "";
            foreach(KeyValuePair<string,int> fields in _lengthOfFields)
            {
                line += "| " + AdjustTokenToLength(_headerName[fields.Key],fields.Value) + " ";
            }
            Console.WriteLine(line + "|");
            PrintTableLine();
        }

        public void PrintHeaderBulkAnalysisFinish(string finishDate)
        {
            PrintTableLine();
            PrintLine("Finished @ " + finishDate+", final number of analyzed APIs: "+_bulkPrintCounter);
        }

        private void PrintTableLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            string line = "";
            foreach (KeyValuePair<string, int> lengthOfField in _lengthOfFields)
            {
                line += "+";
                for(int i = 0; i < lengthOfField.Value + 2; i++)
                {
                    line += "-";
                }
            }
            Console.WriteLine(line + "+");
        }

        public void PrintResult(AnalysisResult analysisResult)
        {
            Console.CursorLeft = 0;

            ConsoleColor lineColor;
            if (!String.IsNullOrWhiteSpace(analysisResult.Get(AnalysisResult.PARAMETER_EXCEPTION)))
            {
                lineColor = ConsoleColor.Red;
            }
            else if (analysisResult.Get(AnalysisResult.PARAMETER_BOOL_HAS_MULTIPLE_PATH_PARAMETER_PER_VARIABLE_PATH_SEGMENT).CompareTo("TRUE") == 0)
            {
                lineColor = ConsoleColor.DarkYellow;
            }
            else if (Int32.Parse(analysisResult.Get(AnalysisResult.PARAMETER_NUM_MISSING_MEDIA_TYPES)) > 0)
            {
                lineColor = ConsoleColor.DarkYellow;
            }
            else
            {
                lineColor = ConsoleColor.White;
            }
            Console.ForegroundColor = lineColor;


            foreach (KeyValuePair<string,int> lengthOfField in _lengthOfFields)
            {
                string value = "";
                if (analysisResult.Has(lengthOfField.Key))
                {
                    value = analysisResult.Get(lengthOfField.Key);
                }
                value = AdjustTokenToLength(value, lengthOfField.Value);

                Console.Write("|");
                if (lengthOfField.Key.CompareTo(AnalysisResult.PARAMETER_SHARE_REACHABLE_RESOURCES) == 0)
                {
                    if(analysisResult.ReachabilityShare > 0.7)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if(analysisResult.ReachabilityShare > 0.4)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }
                Console.Write(" "+value+" ");
            }
            Console.ForegroundColor = lineColor;
            Console.WriteLine("|");
            _bulkPrintCounter++;

            /*
            if(_bulkPrintCounter%10 == 0)
            {
                PrintTableLine();
            }
            */
        }

        public void UpdateStatusBar(double percentage)
        {
            string line = "";
            string label = String.Format("{0:0.00}", percentage) + " %";
            Console.CursorLeft = 0;
            for(int i = 0; i < _tableWidth; i++)
            {
                if(i == (_tableWidth / 2) - 2)
                {
                    line += ((label.Length > 0)? label[0] : ' ');
                }
                else if (i == (_tableWidth / 2) - 1)
                {
                    line += ((label.Length > 1) ? label[1] : ' ');
                }
                else if (i == (_tableWidth / 2))
                {
                    line += ((label.Length > 2) ? label[2] : ' ');
                }
                else if (i == (_tableWidth / 2 +1))
                {
                    line += ((label.Length > 3) ? label[3] : ' ');
                }
                else if (i == (_tableWidth / 2 + 2))
                {
                    line += ((label.Length > 4) ? label[4] : ' ');
                }
                else if (i == (_tableWidth / 2 + 3))
                {
                    line += ((label.Length > 5) ? label[5] : ' ');
                }
                else if (i == (_tableWidth / 2 + 4))
                {
                    line += ((label.Length > 6) ? label[6] : ' ');
                }
                else if(_tableWidth * (percentage/100 ) > i)
                {
                    line += "#";
                }
                else
                {
                    line += " ";
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(line);
        }

        private string AdjustTokenToLength(string token, int length)
        {
            char[] chars = new char[length];
            for(int i = 0; i < chars.Length; i++)
            {
                chars[i] = ' ';
            }
            for(int j = 0; j < token.Length && j < chars.Length; j++)
            {
                chars[j] = token[j];
            }
            return new string(chars);
        }

        #endregion
    }
}
