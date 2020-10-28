/*
 * Usage: Tweet Class used to create Tweet objects and inherits from Message Class.
 * Created by: Valeri Vladimirov 40399682
 * Last modified: 26.10.2020
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkSE
{
    public class Tweet : Message
    {
        private string sender;
        private string text;

        public string Sender
        {
            get { return sender; }
            // Checks if the sender starts with @ and is between 0 and 16 characters.
            set
            {
                if ((value.Length > 0) && (value.Length <= 15) && value.StartsWith("@"))
                {
                    sender = value;
                }
                else
                {
                    throw new ArgumentException("Sender must start with @ and have a max of 15 characters.");
                }
            }
        }

        public string Text
        {
            get { return text; }
            // Checks if the text is between 0 and 140 characters.
            set
            {
                if ((value.Length < 141) && (value.Length > 0))
                {
                    text = value;
                }
                else
                {
                    throw new ArgumentException("Tweet text must be 140 characters or less and not empty.");
                }
            }
        }
    }
}
