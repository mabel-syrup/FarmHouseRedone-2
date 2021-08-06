using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarmHouseRedone
{
    class Strings
    {
        //The string cleanup counts all of these as spaces so that developers can annotate their work if necessary.
        //To keep these as they are, use the escape key \ as in \}.
        internal static List<char> spaceEquivalents = new List<char>
        {
            '|',
            '_',
            '[',
            ']',
            '(',
            ')',
            '{',
            '}',
            ',',
            '.'
        };

        /// <summary>
        /// Cleans up input strings.  To be used with any dev-facing data.
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public static string Cleanup(string dirty)
        {
            string outString = "";
            Logger.Log("Given " + dirty);
            dirty = dirty.Replace("\n", " ");
            dirty = dirty.Replace("\\n", " ");
            Logger.Log("Line breaks cleaned as " + dirty);
            for (int index = 0; index < dirty.Length; index++)
            {
                if(dirty[index] == '\\')
                {
                    index++;
                    outString += dirty[index];
                    continue;
                }
                if (dirty[index] == ' ' && outString.EndsWith(" "))
                    continue;
                if (spaceEquivalents.Contains(dirty[index]))
                {
                    outString += (outString.EndsWith(" ") ? "" : " ");
                    continue;
                }
                outString += dirty[index];
            }
            Logger.Log("Returning " + outString);
            return outString.Trim(' ');
        }

        public static string Fit(string s, SpriteFont font, Vector2 bounds)
        {
            string[] descWords = s.Split(' ');
            string outString = descWords[0] + " ";
            for (int i = 1; i < descWords.Length; i++)
            {
                Vector2 lineSize = font.MeasureString(outString.Split('\n').Last() + descWords[i]);
                if (lineSize.X > bounds.X)
                {
                    if (lineSize.Y + font.MeasureString(outString).Y > bounds.Y)
                    {
                        outString += "...";
                        return outString;
                    }
                    else
                    {
                        outString += "\n";
                    }
                }
                outString += descWords[i] + " ";
            }
            return outString;
        }

        public static string ListToString<T>(List<T> toString)
        {
            string outString = "";
            foreach(object obj in toString)
            {
                outString += obj.ToString() + " ";
            }
            return outString.TrimEnd(' ');
        }
    }
}
