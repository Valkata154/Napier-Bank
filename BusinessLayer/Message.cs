/*
 * Usage: Message Class used to create Message objects.
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
    public class Message
    {
        private string header;
        private string body;

        public string Header
        {
            get { return header; }
            set
            {
                // Checks how long the header is.
                if (value.Length == 10)
                {
                    header = value.ToUpper();
                }
                else
                {
                    throw new ArgumentException("Header must be 10 characters long.");
                }

                // Checks if the header start with the correct character.
                if(value.StartsWith("S") || value.StartsWith("E") || value.StartsWith("T"))
                {
                    header = value.ToUpper();
                }

                else
                {
                    throw new ArgumentException("Header must start with S, E or T.");
                }
            }
        }

        public string Body
        {
            get { return body; }
            set
            {
                // Checks if the body is empty.
                if (0 < value.Length)
                {
                    body = value;
                }
                else
                {
                    throw new ArgumentException("The body shouldn't be empty!");
                }
            }
        }
    }
}
