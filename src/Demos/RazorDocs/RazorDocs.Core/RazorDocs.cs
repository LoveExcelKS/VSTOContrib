﻿using System;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using Office.Contrib;
using Office.Contrib.RibbonFactory;
using Office.Contrib.RibbonFactory.Interfaces;
using Office.Word.Contrib.RibbonFactory;
using CustomTaskPane = Microsoft.Office.Tools.CustomTaskPane;

namespace RazorDocs.Core
{
    [WordRibbonViewModel]
    public class RazorDocs : OfficeViewModelBase, IRibbonViewModel, IRegisterCustomTaskPane
    {
        private bool _panelShown;
        private Document _document;
        private WpfPanelHost _control;
        private CustomTaskPane _razorDocsTaskPane;

        public void Initialised(object context)
        {
            _document = (Document) context;
        }

        public void CurrentViewChanged(object currentView)
        {
            
        }

        public IRibbonUI RibbonUi { get; set; }

        public bool PanelShown
        {
            get { return _panelShown; }
            set
            {
                if (_panelShown == value) return;
                _panelShown = value;
                _razorDocsTaskPane.Visible = value;
                RaisePropertyChanged(()=>PanelShown);
            }
        }

        public void RegisterTaskPanes(Register register)
        {
            //TODO Custom task panes should be 'attached' to new views, but only registered once
            _control = new WpfPanelHost
                           {
                               Child = new RazorDocsPanel
                                           {
                                               DataContext = new RazorDocsPanelViewModel(this)
                                           }
                           };

            _razorDocsTaskPane = register(_control, "RazorDocs");
            _razorDocsTaskPane.Visible = true;
            PanelShown = true;
            _razorDocsTaskPane.VisibleChanged += TwitterTaskPaneVisibleChanged;
            TwitterTaskPaneVisibleChanged(this, EventArgs.Empty);
        }

        public void Cleanup()
        {
            _razorDocsTaskPane.VisibleChanged -= TwitterTaskPaneVisibleChanged;
            if (_control == null) return;

            _control.Dispose();
        }

        private void TwitterTaskPaneVisibleChanged(object sender, EventArgs e)
        {
            _panelShown = _razorDocsTaskPane.Visible;
            RaisePropertyChanged(()=>PanelShown);
        }
    }
}
