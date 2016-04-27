using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Trinity.ItemRules.Core
{
    #region ParseTree

    [Serializable]
    public class ParseErrors : List<ParseError>
    {
    }

    [Serializable]
    public class ParseError
    {
        private string message;
        private int code;
        private int line;
        private int col;
        private int pos;
        private int length;

        public int Code { get { return code; } }
        public int Line { get { return line; } }
        public int Column { get { return col; } }
        public int Position { get { return pos; } }
        public int Length { get { return length; } }
        public string Message { get { return message; } }

        // just for the sake of serialization
        public ParseError()
        {
        }

        public ParseError(string message, int code, ParseNode node)
            : this(message, code, 0, node.Token.StartPos, node.Token.StartPos, node.Token.Length)
        {
        }

        public ParseError(string message, int code, int line, int col, int pos, int length)
        {
            this.message = message;
            this.code = code;
            this.line = line;
            this.col = col;
            this.pos = pos;
            this.length = length;
        }
    }

    // rootlevel of the node tree
    [Serializable]
    public partial class ParseTree : ParseNode
    {
        public ParseErrors Errors;

        public List<Token> Skipped;

        public ParseTree()
            : base(new Token(), "ParseTree")
        {
            Token.Type = TokenType.Start;
            Token.Text = "Root";
            Errors = new ParseErrors();
        }

        public string PrintTree()
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            PrintNode(sb, this, indent);
            return sb.ToString();
        }

        private void PrintNode(StringBuilder sb, ParseNode node, int indent)
        {

            string space = "".PadLeft(indent, ' ');

            sb.Append(space);
            sb.AppendLine(node.Text);

            foreach (ParseNode n in node.Nodes)
                PrintNode(sb, n, indent + 2);
        }

        /// <summary>
        /// this is the start for executing and evaluating the parse tree.
        /// </summary>
        /// <param name="paramlist">additional optional input parameters</param>
        /// <returns>the output of the evaluation function</returns>
        public object Eval(params object[] paramlist)
        {
            return Nodes[0].Eval(this, paramlist);
        }
    }

    [Serializable]
    [XmlInclude(typeof(ParseTree))]
    public partial class ParseNode
    {
        protected string text;
        protected List<ParseNode> nodes;

        public List<ParseNode> Nodes { get { return nodes; } }

        [XmlIgnore] // avoid circular references when serializing
        public ParseNode Parent;
        public Token Token; // the token/rule

        [XmlIgnore] // skip redundant text (is part of Token)
        public string Text
        { // text to display in parse tree 
            get { return text; }
            set { text = value; }
        }

        public virtual ParseNode CreateNode(Token token, string text)
        {
            ParseNode node = new ParseNode(token, text);
            node.Parent = this;
            return node;
        }

        protected ParseNode(Token token, string text)
        {
            this.Token = token;
            this.text = text;
            this.nodes = new List<ParseNode>();
        }

        protected object GetValue(ParseTree tree, TokenType type, int index)
        {
            return GetValue(tree, type, ref index);
        }

        protected object GetValue(ParseTree tree, TokenType type, ref int index)
        {
            object o = null;
            if (index < 0) return o;

            // left to right
            foreach (ParseNode node in nodes)
            {
                if (node.Token.Type == type)
                {
                    index--;
                    if (index < 0)
                    {
                        o = node.Eval(tree);
                        break;
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// This implements the evaluation functionality, cannot be used directly
        /// </summary>
        /// <param name="tree">the parsetree itself</param>
        /// <param name="paramlist">optional input parameters</param>
        /// <returns>a partial result of the evaluation</returns>
        internal object Eval(ParseTree tree, params object[] paramlist)
        {
            object Value = null;

            switch (Token.Type)
            {
                case TokenType.Start:
                    Value = EvalStart(tree, paramlist);
                    break;
                case TokenType.Expr:
                    Value = EvalExpr(tree, paramlist);
                    break;
                case TokenType.SepExpr:
                    Value = EvalSepExpr(tree, paramlist);
                    break;
                case TokenType.OrExpr:
                    Value = EvalOrExpr(tree, paramlist);
                    break;
                case TokenType.AndExpr:
                    Value = EvalAndExpr(tree, paramlist);
                    break;
                case TokenType.CompExpr:
                    Value = EvalCompExpr(tree, paramlist);
                    break;
                case TokenType.AddExpr:
                    Value = EvalAddExpr(tree, paramlist);
                    break;
                case TokenType.MultExpr:
                    Value = EvalMultExpr(tree, paramlist);
                    break;
                case TokenType.Atom:
                    Value = EvalAtom(tree, paramlist);
                    break;

                default:
                    Value = Token.Text;
                    break;
            }
            return Value;
        }

        protected virtual object EvalStart(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.Expr, 0);

            return obj;
        }

        protected virtual object EvalExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.SepExpr, 0);

            return obj;
        }

        protected virtual object EvalSepExpr(ParseTree tree, params object[] paramlist)
        {

            object obj = this.GetValue(tree, TokenType.OrExpr, 0);

            // only do logical AND with bool values
            if (!(obj is bool)) return obj;

            bool value = (bool)obj;
            int i = 1;
            while (this.GetValue(tree, TokenType.OrExpr, i) != null)
            {
                var val = GetValue(tree, TokenType.OrExpr, i++);
                if (val is bool)
                    value &= (bool)val;
            }
            return value;
            //return obj;
        }

        protected virtual object EvalOrExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.AndExpr, 0);

            // only do logical OR with bool values
            if (!(obj is bool)) return obj;

            bool value = (bool)obj;
            int i = 1;
            while (this.GetValue(tree, TokenType.AndExpr, i) != null)
            {
                var unboxVal = GetValue(tree, TokenType.AndExpr, i++);
                if (unboxVal is bool)
                    value |= (bool)unboxVal;
            }
            return value;
        }

        protected virtual object EvalAndExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.CompExpr, 0);

            // only do logical AND with bool values
            if (!(obj is bool)) return obj;

            bool value = (bool)obj;
            int i = 1;
            while (this.GetValue(tree, TokenType.CompExpr, i) != null)
                value &= (bool)this.GetValue(tree, TokenType.CompExpr, i++);
            return value;
        }

        protected virtual object EvalCompExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.AddExpr, 0);

            int i = 1;
            if (obj is bool)
            {
                while (this.GetValue(tree, TokenType.AddExpr, i) != null)
                {
                    if (this.GetValue(tree, TokenType.EQUAL, i - 1) != null)
                        return (bool)obj == (bool)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.NOTEQUAL, i - 1) != null)
                        return (bool)obj != (bool)this.GetValue(tree, TokenType.AddExpr, i++);
                    Console.WriteLine("Major Mistake comparator not allowed! #133");
                }
            }
            else if (obj is float)
            {
                while (this.GetValue(tree, TokenType.AddExpr, i) != null)
                {
                    if (this.GetValue(tree, TokenType.EQUAL, i - 1) != null)
                        return (float)obj == (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.NOTEQUAL, i - 1) != null)
                        return (float)obj != (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.SMALLEQ, i - 1) != null)
                        return (float)obj <= (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.GREATEQ, i - 1) != null)
                        return (float)obj >= (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.SMALLTH, i - 1) != null)
                        return (float)obj < (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.GREATTH, i - 1) != null)
                        return (float)obj > (float)this.GetValue(tree, TokenType.AddExpr, i++);
                    Console.WriteLine("Major Mistake comparator not allowed! #132");
                }
            }
            else if (obj is string)
            {
                while (this.GetValue(tree, TokenType.AddExpr, i) != null)
                {
                    if (this.GetValue(tree, TokenType.EQUAL, i - 1) != null)
                        return (string)obj == (string)this.GetValue(tree, TokenType.AddExpr, i++);
                    else if (this.GetValue(tree, TokenType.NOTEQUAL, i - 1) != null)
                        return (string)obj != (string)this.GetValue(tree, TokenType.AddExpr, i++);
                    Console.WriteLine("Major Mistake comparator not allowed! #133");
                }
            }

            return obj;
        }

        protected virtual object EvalAddExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.MultExpr, 0);

            // only do addition and substraction for floating numbers
            if (!(obj is float) && !(obj is bool && this.GetValue(tree, TokenType.PLUS, 0) != null)) return obj;

            float value = Convert.ToSingle(obj);
            int i = 1;
            while (this.GetValue(tree, TokenType.MultExpr, i) != null)
            {
                if (this.GetValue(tree, TokenType.PLUS, i - 1) != null)
                    value += Convert.ToSingle(this.GetValue(tree, TokenType.MultExpr, i++));
                else
                    value -= (float)this.GetValue(tree, TokenType.MultExpr, i++);
            }
            return value;
        }

        protected virtual object EvalMultExpr(ParseTree tree, params object[] paramlist)
        {
            object obj = this.GetValue(tree, TokenType.Atom, 0);

            // only do multiplication and divisions for floating numbers
            if (!(obj is float)) return obj;

            float value = (float)obj;
            int i = 1;
            while (this.GetValue(tree, TokenType.Atom, i) != null)
            {
                if (this.GetValue(tree, TokenType.MULT, i - 1) != null)
                    value *= (float)this.GetValue(tree, TokenType.Atom, i++);
                else // if (this.GetValue(tree, TokenType.DIV, i - 1) != null) 
                    value /= (float)this.GetValue(tree, TokenType.Atom, i++);
            }
            return value;
        }

        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist)
        {
            if (this.GetValue(tree, TokenType.VARIABLE, 0) != null)
                return getAtomValue(tree, TokenType.VARIABLE, 0);

            else if (this.GetValue(tree, TokenType.STRING, 0) != null)
                return getAtomValue(tree, TokenType.STRING, 0);

            else if (this.GetValue(tree, TokenType.NUMBER, 0) != null)
                return getAtomValue(tree, TokenType.NUMBER, 0);

            else if (this.GetValue(tree, TokenType.BOOLEAN, 0) != null)
                return getAtomValue(tree, TokenType.BOOLEAN, 0);

            else
                return this.GetValue(tree, TokenType.Expr, 0);
        }

        protected virtual object getAtomValue(ParseTree tree, TokenType tokenType, int index)
        {
            object obj = this.GetValue(tree, tokenType, 0);

            switch (tokenType)
            {
                case TokenType.VARIABLE:
                    object value;
                    //if (Interpreter.itemDic.TryGetValue(obj.ToString(), out value))
                    //    return value;
                    if (Interpreter.getVariableValue(obj.ToString(), out value))
                        return value;
                    break;

                case TokenType.STRING:
                    return obj.ToString().Replace("\"", "");

                case TokenType.NUMBER:
                    float resultFloat;
                    if (Single.TryParse(obj.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out resultFloat))
                        return resultFloat;
                    break;

                case TokenType.BOOLEAN:
                    bool resultBool;
                    if (Boolean.TryParse(obj.ToString(), out resultBool))
                        return resultBool;
                    break;
            }
            tree.Errors.Add(new ParseError("TryGetValue " + tokenType + " failed '" + obj.ToString() + "' check this key!", 111, 0, 0, 0, 0));
            return null;
        }

    }

    #endregion ParseTree
}
