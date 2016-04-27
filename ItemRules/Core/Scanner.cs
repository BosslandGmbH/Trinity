using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Trinity.ItemRules.Core
{
    #region Scanner

    public partial class Scanner
    {
        public string Input;
        public int StartPos = 0;
        public int EndPos = 0;
        public int CurrentLine;
        public int CurrentColumn;
        public int CurrentPosition;
        public List<Token> Skipped; // tokens that were skipped
        public Dictionary<TokenType, Regex> Patterns;

        private Token LookAheadToken;
        private List<TokenType> Tokens;
        private List<TokenType> SkipList; // tokens to be skipped

        public Scanner()
        {
            Regex regex;
            Patterns = new Dictionary<TokenType, Regex>();
            Tokens = new List<TokenType>();
            LookAheadToken = null;
            Skipped = new List<Token>();

            SkipList = new List<TokenType>();
            SkipList.Add(TokenType.WHITESPACE);

            regex = new Regex(@"^\s*$", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOF, regex);
            Tokens.Add(TokenType.EOF);

            regex = new Regex(@"(\[@?[A-Z]+[%]?\])(\.[A-Za-z]+)?", RegexOptions.Compiled);
            Patterns.Add(TokenType.VARIABLE, regex);
            Tokens.Add(TokenType.VARIABLE);

            //regex = new Regex(@"\""[0-9A-Za-z' :\-]*\""", RegexOptions.Compiled);
            // fix for all language (chinese,russian, etc.)
            regex = new Regex(@"\""([^\""]*)\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.STRING, regex);
            Tokens.Add(TokenType.STRING);

            regex = new Regex(@"[0-9]*(\.[0-9]+)?", RegexOptions.Compiled);
            Patterns.Add(TokenType.NUMBER, regex);
            Tokens.Add(TokenType.NUMBER);

            regex = new Regex(@"[Tt][Rr][Uu][Ee]|[Ff][Aa][Ll][Ss][Ee]", RegexOptions.Compiled);
            Patterns.Add(TokenType.BOOLEAN, regex);
            Tokens.Add(TokenType.BOOLEAN);

            regex = new Regex(@"#", RegexOptions.Compiled);
            Patterns.Add(TokenType.SEPARATOR, regex);
            Tokens.Add(TokenType.SEPARATOR);

            regex = new Regex(@"\|\|", RegexOptions.Compiled);
            Patterns.Add(TokenType.OR, regex);
            Tokens.Add(TokenType.OR);

            regex = new Regex(@"&&", RegexOptions.Compiled);
            Patterns.Add(TokenType.AND, regex);
            Tokens.Add(TokenType.AND);

            regex = new Regex(@"==", RegexOptions.Compiled);
            Patterns.Add(TokenType.EQUAL, regex);
            Tokens.Add(TokenType.EQUAL);

            regex = new Regex(@"!=", RegexOptions.Compiled);
            Patterns.Add(TokenType.NOTEQUAL, regex);
            Tokens.Add(TokenType.NOTEQUAL);

            regex = new Regex(@"<=", RegexOptions.Compiled);
            Patterns.Add(TokenType.SMALLEQ, regex);
            Tokens.Add(TokenType.SMALLEQ);

            regex = new Regex(@">=", RegexOptions.Compiled);
            Patterns.Add(TokenType.GREATEQ, regex);
            Tokens.Add(TokenType.GREATEQ);

            regex = new Regex(@"<", RegexOptions.Compiled);
            Patterns.Add(TokenType.SMALLTH, regex);
            Tokens.Add(TokenType.SMALLTH);

            regex = new Regex(@">", RegexOptions.Compiled);
            Patterns.Add(TokenType.GREATTH, regex);
            Tokens.Add(TokenType.GREATTH);

            regex = new Regex(@"\+", RegexOptions.Compiled);
            Patterns.Add(TokenType.PLUS, regex);
            Tokens.Add(TokenType.PLUS);

            regex = new Regex(@"-", RegexOptions.Compiled);
            Patterns.Add(TokenType.MINUS, regex);
            Tokens.Add(TokenType.MINUS);

            regex = new Regex(@"\*", RegexOptions.Compiled);
            Patterns.Add(TokenType.MULT, regex);
            Tokens.Add(TokenType.MULT);

            regex = new Regex(@"\/", RegexOptions.Compiled);
            Patterns.Add(TokenType.DIV, regex);
            Tokens.Add(TokenType.DIV);

            regex = new Regex(@"\(", RegexOptions.Compiled);
            Patterns.Add(TokenType.BROPEN, regex);
            Tokens.Add(TokenType.BROPEN);

            regex = new Regex(@"\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRCLOSE, regex);
            Tokens.Add(TokenType.BRCLOSE);

            regex = new Regex(@"\s+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHITESPACE, regex);
            Tokens.Add(TokenType.WHITESPACE);


        }

        public void Init(string input)
        {
            Input = input;
            StartPos = 0;
            EndPos = 0;
            CurrentLine = 0;
            CurrentColumn = 0;
            CurrentPosition = 0;
            LookAheadToken = null;
        }

        public Token GetToken(TokenType type)
        {
            Token t = new Token(this.StartPos, this.EndPos);
            t.Type = type;
            return t;
        }

        /// <summary>
        /// executes a lookahead of the next token
        /// and will advance the scan on the input string
        /// </summary>
        /// <returns></returns>
        public Token Scan(params TokenType[] expectedtokens)
        {
            Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
            LookAheadToken = null; // reset lookahead token, so scanning will continue
            StartPos = tok.EndPos;
            EndPos = tok.EndPos; // set the tokenizer to the new scan position
            return tok;
        }

        /// <summary>
        /// returns token with longest best match
        /// </summary>
        /// <returns></returns>
        public Token LookAhead(params TokenType[] expectedtokens)
        {
            int i;
            int startpos = StartPos;
            Token tok = null;
            List<TokenType> scantokens;


            // this prevents double scanning and matching
            // increased performance
            if (LookAheadToken != null
                && LookAheadToken.Type != TokenType._UNDETERMINED_
                && LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

            // if no scantokens specified, then scan for all of them (= backward compatible)
            if (expectedtokens.Length == 0)
                scantokens = Tokens;
            else
            {
                scantokens = new List<TokenType>(expectedtokens);
                scantokens.AddRange(SkipList);
            }

            do
            {

                int len = -1;
                TokenType index = (TokenType)int.MaxValue;
                string input = Input.Substring(startpos);

                tok = new Token(startpos, EndPos);

                for (i = 0; i < scantokens.Count; i++)
                {
                    Regex r = Patterns[scantokens[i]];
                    Match m = r.Match(input);
                    if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len)))
                    {
                        len = m.Length;
                        index = scantokens[i];
                    }
                }

                if (index >= 0 && len >= 0)
                {
                    tok.EndPos = startpos + len;
                    tok.Text = Input.Substring(tok.StartPos, len);
                    tok.Type = index;
                }
                else if (tok.StartPos < tok.EndPos - 1)
                {
                    tok.Text = Input.Substring(tok.StartPos, 1);
                }

                if (SkipList.Contains(tok.Type))
                {
                    startpos = tok.EndPos;
                    Skipped.Add(tok);
                }
                else
                {
                    // only assign to non-skipped tokens
                    tok.Skipped = Skipped; // assign prior skips to this token
                    Skipped = new List<Token>(); //reset skips
                }
            }
            while (SkipList.Contains(tok.Type));

            LookAheadToken = tok;
            return tok;
        }
    }

    #endregion

    #region Token

    public enum TokenType
    {

        //Non terminal tokens:
        _NONE_ = 0,
        _UNDETERMINED_ = 1,

        //Non terminal tokens:
        Start = 2,
        Expr = 3,
        SepExpr = 4,
        OrExpr = 5,
        AndExpr = 6,
        CompExpr = 7,
        AddExpr = 8,
        MultExpr = 9,
        Atom = 10,

        //Terminal tokens:
        EOF = 11,
        VARIABLE = 12,
        STRING = 13,
        NUMBER = 14,
        BOOLEAN = 15,
        SEPARATOR = 16,
        OR = 17,
        AND = 18,
        EQUAL = 19,
        NOTEQUAL = 20,
        SMALLEQ = 21,
        GREATEQ = 22,
        SMALLTH = 23,
        GREATTH = 24,
        PLUS = 25,
        MINUS = 26,
        MULT = 27,
        DIV = 28,
        BROPEN = 29,
        BRCLOSE = 30,
        WHITESPACE = 31
    }

    public class Token
    {
        // contains all prior skipped symbols

        public int StartPos { get; set; }

        public int Length
        {
            get { return EndPos - StartPos; }
        }

        public int EndPos { get; set; }

        public string Text { get; set; }

        public List<Token> Skipped { get; set; }

        public object Value { get; set; }

        [XmlAttribute]
        public TokenType Type;

        [DebuggerStepThrough]
        public Token()
            : this(0, 0)
        {
        }
        [DebuggerStepThrough]
        public Token(int start, int end)
        {
            Type = TokenType._UNDETERMINED_;
            StartPos = start;
            EndPos = end;
            Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
            Value = null;
        }

        [DebuggerStepThrough]
        public void UpdateRange(Token token)
        {
            if (token.StartPos < StartPos) StartPos = token.StartPos;
            if (token.EndPos > EndPos) EndPos = token.EndPos;
        }

        public override string ToString()
        {
            if (Text != null)
                return Type.ToString() + " '" + Text + "'";
            else
                return Type.ToString();
        }
    }

    #endregion
}
