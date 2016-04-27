

namespace Trinity.ItemRules.Core
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

        public ParseTree Parse(string input)
        {
            tree = new ParseTree();
            return Parse(input, tree);
        }

        public ParseTree Parse(string input, ParseTree tree)
        {
            scanner.Init(input);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);


            
            tok = scanner.LookAhead(TokenType.VARIABLE, TokenType.STRING, TokenType.NUMBER, TokenType.BOOLEAN, TokenType.BROPEN);
            if (tok.Type == TokenType.VARIABLE
                || tok.Type == TokenType.STRING
                || tok.Type == TokenType.NUMBER
                || tok.Type == TokenType.BOOLEAN
                || tok.Type == TokenType.BROPEN)
            {
                ParseExpr(node);
            }

            
            tok = scanner.Scan(TokenType.EOF);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.EOF) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EOF.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseExpr(ParseNode parent)
        {
            //Token tok;
            //ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Expr), "Expr");
            parent.Nodes.Add(node);

            ParseSepExpr(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseSepExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.SepExpr), "SepExpr");
            parent.Nodes.Add(node);

            ParseOrExpr(node);

            tok = scanner.LookAhead(TokenType.SEPARATOR);
            while (tok.Type == TokenType.SEPARATOR)
            {
                tok = scanner.Scan(TokenType.SEPARATOR);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.SEPARATOR) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SEPARATOR.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }

                ParseOrExpr(node);
            tok = scanner.LookAhead(TokenType.SEPARATOR);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseOrExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.OrExpr), "OrExpr");
            parent.Nodes.Add(node);

            ParseAndExpr(node);

            tok = scanner.LookAhead(TokenType.OR);
            while (tok.Type == TokenType.OR)
            {
                tok = scanner.Scan(TokenType.OR);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.OR) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.OR.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }

                ParseAndExpr(node);
            tok = scanner.LookAhead(TokenType.OR);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAndExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.AndExpr), "AndExpr");
            parent.Nodes.Add(node);

            ParseCompExpr(node);

            tok = scanner.LookAhead(TokenType.AND);
            while (tok.Type == TokenType.AND)
            {
                tok = scanner.Scan(TokenType.AND);
                n = node.CreateNode(tok, tok.ToString() );
                node.Token.UpdateRange(tok);
                node.Nodes.Add(n);
                if (tok.Type != TokenType.AND) {
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.AND.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                    return;
                }
                
                ParseCompExpr(node);
            tok = scanner.LookAhead(TokenType.AND);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseCompExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.CompExpr), "CompExpr");
            parent.Nodes.Add(node);

            ParseAddExpr(node);

            tok = scanner.LookAhead(TokenType.EQUAL, TokenType.NOTEQUAL, TokenType.SMALLEQ, TokenType.GREATEQ, TokenType.SMALLTH, TokenType.GREATTH);
            if (tok.Type == TokenType.EQUAL
                || tok.Type == TokenType.NOTEQUAL
                || tok.Type == TokenType.SMALLEQ
                || tok.Type == TokenType.GREATEQ
                || tok.Type == TokenType.SMALLTH
                || tok.Type == TokenType.GREATTH)
            {

                
                tok = scanner.LookAhead(TokenType.EQUAL, TokenType.NOTEQUAL, TokenType.SMALLEQ, TokenType.GREATEQ, TokenType.SMALLTH, TokenType.GREATTH);
                switch (tok.Type)
                {
                    case TokenType.EQUAL:
                        tok = scanner.Scan(TokenType.EQUAL);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.EQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.EQUAL.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.NOTEQUAL:
                        tok = scanner.Scan(TokenType.NOTEQUAL);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.NOTEQUAL) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NOTEQUAL.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.SMALLEQ:
                        tok = scanner.Scan(TokenType.SMALLEQ);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.SMALLEQ) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SMALLEQ.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.GREATEQ:
                        tok = scanner.Scan(TokenType.GREATEQ);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.GREATEQ) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.GREATEQ.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.SMALLTH:
                        tok = scanner.Scan(TokenType.SMALLTH);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.SMALLTH) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SMALLTH.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.GREATTH:
                        tok = scanner.Scan(TokenType.GREATTH);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.GREATTH) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.GREATTH.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }
               
                ParseAddExpr(node);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAddExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.AddExpr), "AddExpr");
            parent.Nodes.Add(node);

            ParseMultExpr(node);

            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS);
            while (tok.Type == TokenType.PLUS
                || tok.Type == TokenType.MINUS)
            {

                tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS);
                switch (tok.Type)
                {
                    case TokenType.PLUS:
                        tok = scanner.Scan(TokenType.PLUS);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.PLUS) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.PLUS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.MINUS:
                        tok = scanner.Scan(TokenType.MINUS);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.MINUS) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MINUS.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }

                
                ParseMultExpr(node);
            tok = scanner.LookAhead(TokenType.PLUS, TokenType.MINUS);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseMultExpr(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.MultExpr), "MultExpr");
            parent.Nodes.Add(node);


            
            ParseAtom(node);

            
            tok = scanner.LookAhead(TokenType.MULT, TokenType.DIV);
            while (tok.Type == TokenType.MULT
                || tok.Type == TokenType.DIV)
            {

                
                tok = scanner.LookAhead(TokenType.MULT, TokenType.DIV);
                switch (tok.Type)
                {
                    case TokenType.MULT:
                        tok = scanner.Scan(TokenType.MULT);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.MULT) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.MULT.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    case TokenType.DIV:
                        tok = scanner.Scan(TokenType.DIV);
                        n = node.CreateNode(tok, tok.ToString() );
                        node.Token.UpdateRange(tok);
                        node.Nodes.Add(n);
                        if (tok.Type != TokenType.DIV) {
                            tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.DIV.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                            return;
                        }
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                        break;
                }

                
                ParseAtom(node);
            tok = scanner.LookAhead(TokenType.MULT, TokenType.DIV);
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAtom(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Atom), "Atom");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.VARIABLE, TokenType.STRING, TokenType.NUMBER, TokenType.BOOLEAN, TokenType.BROPEN);
            switch (tok.Type)
            {
                case TokenType.VARIABLE:
                    tok = scanner.Scan(TokenType.VARIABLE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.VARIABLE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.VARIABLE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.STRING:
                    tok = scanner.Scan(TokenType.STRING);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.STRING) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.STRING.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.NUMBER) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.BOOLEAN:
                    tok = scanner.Scan(TokenType.BOOLEAN);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BOOLEAN) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BOOLEAN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                case TokenType.BROPEN:

                    
                    tok = scanner.Scan(TokenType.BROPEN);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BROPEN) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BROPEN.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }

                    
                    ParseExpr(node);

                    
                    tok = scanner.Scan(TokenType.BRCLOSE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.BRCLOSE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.BRCLOSE.ToString(), 0x1001, 0, tok.StartPos, tok.StartPos, tok.Length));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found.", 0x0002, 0, tok.StartPos, tok.StartPos, tok.Length));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}
