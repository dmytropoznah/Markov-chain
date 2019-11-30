using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markov_Chain_Sentence_Generator
{
    class Markov
    {
        private string text;  
        private Dictionary<string, Trigram> model;  
        private bool trained;

        static Random random = new Random();

        public Markov(string text)
        {
            this.text = text;
            model = new Dictionary<string, Trigram>();
            this.trained = false;
        }

        public void Train()
        {
            this.trained = false;
            text = CleanText(text);
            string[] sentences = text.Split('.');
            if (sentences.Length < 2) 
            {
                throw new FormatException("The text provided has no periods('.'). Please use a text with sentences seperated with periods.");
            }
            for (int i = 0; i != sentences.Length; i++)
            {
                string[] words = sentences[i].Split(' ');
                if (words.Length < 3) { break; } 
                for (int j = 0; j != words.Length - 2; j++)
                {
                    string index = words[j] + words[j + 1];
                    if (!model.ContainsKey(index))
                    {
                        model[index] = new Trigram(words[j], words[j+1]);  
                    }
                    model[index].Add(words[j + 2]); 
                }
            }
            trained = true;
        }   

        public string GenerateText(int length = 20)
        {
            if (!trained)
            {
                throw new Exception("The model has not been trained yet.");
            }
            List<string> keyList = new List<string>(model.Keys);  
            List<string> sentence = new List<string>();  
            string[] index = model[keyList[random.Next(keyList.Count)]].prefixWords; 
            sentence.Add(index[0]); 
            sentence.Add(index[1]);
            for (int i = 1; i != length; i++)
            {
                index[0] = sentence[i - 1]; 
                index[1] = sentence[i];
                try
                {  
                    List<string> suffixes = model[index[0] + index[1]].suffixes;
                    string choice = suffixes[random.Next(suffixes.Count)];
                    sentence.Add(choice);  
                }
                catch (KeyNotFoundException)  
                {
                    index = model[keyList[random.Next(keyList.Count)]].prefixWords;
                    sentence.Add(index[0]); 
                    sentence.Add(index[1]);
                }
            }
            return string.Join(" ", sentence);
        }  

        private string CleanText(string text)
        {   
            string cleaned = text.Replace("...", "");
            cleaned = cleaned.Replace("\n", "");
            return cleaned;
        }   

    }   
}
