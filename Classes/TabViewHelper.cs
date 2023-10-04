

    using DevExpress.XtraBars.Docking2010.Views.Tabbed;
    using DevExpress.XtraBars.Docking2010.Views;
    using System;
    using System.Windows.Forms;
    using DevExpress.XtraEditors;

    namespace OrderManager.Classes
    {
        public class TabbedViewHelper
        {
            private TabbedView _tabbedView;


            public TabbedViewHelper(TabbedView tabbedView)
            {
                _tabbedView = tabbedView;
            }

 
            public bool AddAndHideDocument<T>(Func<T> createFunc) where T : Form
            {
                try
                {
                    T documentForm = createFunc();
                    documentForm.Tag = typeof(T);
                    BaseDocument document = _tabbedView.AddDocument(documentForm);
                    document.Form.Shown += (sender, e) => { documentForm.Hide(); };
                    return true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"An error occurred while adding and hiding the document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

        public void ShowDocumentByType<T>(Func<T> createFunc) where T : Form
            {
                try
                {
                    BaseDocument documentToShow = null;

                    foreach (BaseDocument document in _tabbedView.Documents)
                    {
                        if (document.Form.Tag is Type type && type == typeof(T))
                        {
                            documentToShow = document;
                            break;
                        }
                    }

                    if (documentToShow == null || documentToShow.Form.IsDisposed)
                    {
                        T newForm = createFunc();
                        newForm.Tag = typeof(T);
                        documentToShow = _tabbedView.AddDocument(newForm);
                    }

                    documentToShow.Form.Show();
                    _tabbedView.Controller.Activate(documentToShow);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"An error occurred while showing the document: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }


