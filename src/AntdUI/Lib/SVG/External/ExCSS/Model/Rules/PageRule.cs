﻿// THIS FILE IS PART OF ExCSS PROJECT
// THE ExCSS PROJECT IS AN OPENSOURCE LIBRARY LICENSED UNDER THE MIT License.
// COPYRIGHT (C) TylerBrinks. ALL RIGHTS RESERVED.
// GITHUB: https://github.com/TylerBrinks/ExCSS

using AntdUI.Svg.ExCSS.Model;
using AntdUI.Svg.ExCSS.Model.Extensions;

// ReSharper disable once CheckNamespace
namespace AntdUI.Svg.ExCSS
{
    public class PageRule : RuleSet, ISupportsSelector, ISupportsDeclarations
    {
        private readonly StyleDeclaration _declarations;
        private BaseSelector _selector;
        private string _selectorText;

        public PageRule()
        {
            _declarations = new StyleDeclaration();
            RuleType = RuleType.Page;
        }

        internal PageRule AppendRule(Property rule)
        {
            _declarations.Properties.Add(rule);
            return this;
        }

        public BaseSelector Selector
        {
            get { return _selector; }
            set
            {
                _selector = value;
                _selectorText = value.ToString();
            }
        }

        public StyleDeclaration Declarations
        {
            get { return _declarations; }
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            var pseudo = string.IsNullOrEmpty(_selectorText)
                             ? ""
                             : ":" + _selectorText;

            var declarations = _declarations.ToString(friendlyFormat, indentation);//.TrimFirstLine();

            return ("@page " + pseudo + "{").NewLineIndent(friendlyFormat, indentation) +
                declarations +
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
