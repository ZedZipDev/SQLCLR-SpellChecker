using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace SpellChecker
{
    public class SpellCheck
    {
        private string alphabet = "abcdefghijklmnopqrstuvwxyz";
        private bool m_bInitialized = false;
        public int totalCount = 0;
        public SqlConnection m_connection;
        public bool IsInitialized()
        {
            return m_bInitialized;
        }
        public SpellCheck()
        {
        }
        public SpellCheck(SqlConnection connection)
        {
            m_connection = connection;
            InitObject();
        }
        public bool InitObject()
        {
            nWords = wordsSql();
            m_bInitialized = true;
            return true;
        }
        delegate TRet Func<T1, TRet>(T1 arg);
        Func<string, int> nWords;
        Func<string,int> wordsSql()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            try
            {
                if (m_connection.State != ConnectionState.Open) m_connection.Open();

                SqlCommand sqlGetWords = m_connection.CreateCommand();
                sqlGetWords.CommandType = CommandType.Text;
                sqlGetWords.CommandText = "SELECT [total],[word] FROM [dbo].[checkerwords] with(nolock)";
                using (SqlDataReader reader = sqlGetWords.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        totalCount++;
                        int wordCount = 0;
                        try
                        {
                            wordCount = Convert.ToInt32(reader["total"].ToString());
                        }
                        catch (InvalidCastException)
                        {
                            wordCount = 0;
                        }
                        d[reader["word"].ToString()] = wordCount;
                    }
                }
            }
            catch (Exception)
            {
            }
            return delegate(string w)
            {
                return d.ContainsKey(w) ? d[w] : 1;
            };
        }
        List<string> edits1(string word)
        {
            List<string> ret = new List<string>();
            // Deletion
            for (int i = 0; i != word.Length; ++i)
            {
                string candidate = word.Substring(0, i) + word.Substring(i + 1);
                if (!String.IsNullOrEmpty(candidate) && !ret.Contains(candidate))
                    if (!ret.Contains(candidate)) { ret.Add(candidate); }

            }
            // Transposition
            for (int i = 0; i != word.Length - 1; ++i)
            {
                string candidate = word.Substring(0, i) + word.Substring(i + 1, 1) +
                    word.Substring(i, 1) + word.Substring(i + 2);
                if (!String.IsNullOrEmpty(candidate) && !ret.Contains(candidate))
                    ret.Add(candidate);
            }
            // Alteration
            for (int i = 0; i != word.Length; ++i)
            {
                foreach (char c in alphabet)
                {
                    string candidate = word.Substring(0, i) + c.ToString() + word.Substring(i + 1);
                    if (!String.IsNullOrEmpty(candidate) && !ret.Contains(candidate))
                        ret.Add(candidate);                     
                }
            }
            // Insertion
            for (int i = 0; i != word.Length + 1; ++i)
            {
                foreach (char c in alphabet)
                {
                    string candidate = word.Substring(0, i) + c.ToString() +
                        word.Substring(i);
                    if (!String.IsNullOrEmpty(candidate) && !ret.Contains(candidate))
                        ret.Add(candidate);
                }
            }

            return ret;
        }

        List<string> known_edits2(string word)
        {
            List<string> e1 = edits1(word);
            List<string> ret = new List<string>();
            foreach (string w in e1)
            {
                foreach (string w2 in edits1(w))
                {
                    if (nWords(w2) > 1 && ret.IndexOf(w2) < 0)  ret.Add(w2);
                }
            }

            return ret;
        }

        List<string> known(List<string> words)
        {
            List<string> ret = new List<string>();
            foreach (string w in words)
            {
                if (nWords(w) > 1)   ret.Add(w);
            }

            return ret;
        }
        public int known(string w)
        {
            int n = nWords(w);
            return n;
        }
        public string correct(string word)
        {
            int mm = known(word);
            if (mm > 1) return word;

            List<string> candidates = known(edits1(word));
            if (candidates.Count == 0) candidates = known_edits2(word);
            int max = 0;
            string correctWord = word;
            foreach (string w in candidates)
            {
                int n = nWords(w);
                if (n > max)
                {
                    max = n;
                    correctWord = w;
                }
            }
            return correctWord;
        }
        public ArrayList correct(string word, ref string correctWord)
        {
            ArrayList strArray = new ArrayList();

            if (known(new List<string>(new string[] { word })).Count > 0)
                return strArray;
            List<string> candidates = known(edits1(word));
            if (candidates.Count == 0)
            {
                candidates = known_edits2(word);
            }

            int max = 0;
            correctWord = word;
            foreach (string w in candidates)
            {
                int n = nWords(w);
                if (n > max)
                {
                    max = n;
                    correctWord = w;
                }
                strArray.Add(w+":"+n.ToString());
            }
            return strArray;
        }
    }
}