using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AGLibrary.Files;

namespace DigDesNote.UI.WPF
{
    public class AdmClass
    { 
        public String _loginSetPath = "adm//loginset.json";
        public String _startDomain = "http://localhost:41606/api/";

        public void ReadSet()
        {
            try
            {
                FileWork.ReadDataJson<AdmClass>(out AdmClass adm, "config.json");
                if (adm == null) throw new Exception();
                this._loginSetPath = adm._loginSetPath;
                this._startDomain = adm._startDomain;
            }
            catch (Exception ex)
            {
                SaveSet();
            }
        }
        public void SaveSet()
        {
            try
            {
                FileWork.SaveDataJson<AdmClass>(this, "config.json");
            }
            catch(Exception ex)
            {

            }
        }
    }
}