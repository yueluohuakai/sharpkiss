// switchcase.cs
//
// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Text;

namespace Kiss.Web.Utils.ajaxmin
{
    public sealed class SwitchCase : AstNode
    {
        private AstNode m_caseValue;
        private Block m_statements;

        public AstNode CaseValue
        {
            get { return m_caseValue; }
            set
            {
                m_caseValue.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_caseValue = value;
                m_caseValue.IfNotNull(n => n.Parent = this);
            }
        }

        public Block Statements
        {
            get { return m_statements; }
            set
            {
                m_statements.IfNotNull(n => n.Parent = (n.Parent == this) ? null : n.Parent);
                m_statements = value;
                m_statements.IfNotNull(n => n.Parent = this);
            }
        }

        internal bool IsDefault
        {
            get { return (CaseValue == null); }
        }

        public Context ColonContext { get; set; }

        public SwitchCase(Context context, JSParser parser)
            : base(context, parser)
        {
        }

        public override void Accept(IVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
        }

        public override IEnumerable<AstNode> Children
        {
            get
            {
                return EnumerateNonNullNodes(CaseValue, Statements);
            }
        }

        public override bool ReplaceChild(AstNode oldNode, AstNode newNode)
        {
            if (CaseValue == oldNode)
            {
                CaseValue = newNode;
                return true;
            }
            if (Statements == oldNode)
            {
                var newBlock = newNode as Block;
                if (newNode == null || newBlock != null)
                {
                    Statements = newBlock;
                    return true;
                }
            }
            return false;
        }

        internal override bool RequiresSeparator
        {
            get
            {
                // no statements doesn't require a separator.
                // otherwise only if statements require it
                if (Statements == null || Statements.Count == 0)
                {
                    return false;
                }

                return Statements[Statements.Count - 1].RequiresSeparator;
            }
        }
    }
}