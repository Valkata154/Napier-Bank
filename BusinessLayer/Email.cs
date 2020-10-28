/*
 * Usage: Email Class used to create Email objects and inherits from Message Class.
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
    public class Email : Message
    {
        private string sender;
        private string subject;
        private string text;

        public string Sender
        {
            get { return sender; }
            set
            {
                sender = value;
            }
        }

        public string Subject
        {
            get { return subject; }
            // Checks if the subject is between 0 and 20 characters.
            set
            {
                if ((value.Length > 0) && (value.Length <= 20))
                {
                    subject = value;
                }
                else
                {
                    throw new Exception("Subject must be less than or equal to 20 characters, but not empty.");
                }
            }
        }

        public string Text
        {
            get { return text; }
            // Checks if the text is less than or equal to 1028 characters.
            set
            {
                if (value.Length <= 1028)
                {
                    text = value;
                }
                else
                {
                    throw new Exception("Text must less than or equal to 1028 characters.");
                }
            }
        }
    }
}
