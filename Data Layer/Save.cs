/*
 * Usage: Save Class used to save the messages to a json file using Serialization and newtonsoft nuget package.
 * The file will be located at 
 * Created by: Valeri Vladimirov 40399682
 * Last modified: 29.10.2020
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CourseworkSE
{
    [Serializable()]
    public class Save
    {
        public void Serialize(List<Message> messages)
        {
            using (StreamWriter f = File.CreateText(@"../../../JSON/messages.json"))
            {
                JsonSerializer s = new JsonSerializer();
                s.Serialize(f, messages);
            }
        }
    }
}
