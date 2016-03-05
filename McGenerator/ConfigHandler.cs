using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace McGenerator
{
    class ConfigHandler
    {
        private Logger logger;
        XmlDocument cfgdoc;

        public ConfigHandler(Logger logger_)
        {
            logger = logger_;

            try
            {
                cfgdoc = new XmlDocument();
                cfgdoc.Load("config.xml");
            }
            catch( SystemException ex )
            {
                logger.Log(ex.Message, LogCategory.lcError);
            }
            logger.Log("Created ConfigHandler", LogCategory.lcMessage);
        }

        public string[] GetAllTypes()
        {
            try
            {
                XmlNodeList elemList = cfgdoc.GetElementsByTagName("camera");
                string[] result = new string[elemList.Count];

                for (int i = 0; i < elemList.Count; i++)
                {
                    string attrType = elemList[i].Attributes["type"].Value;
                    string attrMac = elemList[i].Attributes["mac"].Value;
                    logger.Log(string.Format("{0}: {1}", attrType, attrMac), LogCategory.lcMessage);
                    result[i] = attrType;
                }
                return result;
            }
            catch( SystemException ex)
            {
                logger.Log(ex.Message, LogCategory.lcError);
                return null;
            }
        }

        public string GetDefaultMAC(string type)
        {
            try
            {
                XmlNodeList elemList = cfgdoc.GetElementsByTagName("camera");

                for (int i = 0; i < elemList.Count; i++)
                {
                    if (elemList[i].Attributes["type"].Value == type)
                    {
                        return elemList[i].Attributes["mac"].Value;
                    }
                }
            }
            catch (SystemException ex)
            {
                logger.Log(ex.Message, LogCategory.lcError);
            }

            return string.Empty;
        }

        public static Int64 MACStringToInt64(string smac)
        {
            Int64 result = 0;

            if(smac != string.Empty)
            {
                try
                {
                    string value = smac.Replace(":", "");
                    result = Convert.ToInt64(value, 16);
                }
                catch {}
            }
            return result;
        }

        public static string MACInt64ToString(Int64 imac)
        {
            if(imac != 0)
            {
                string smac = string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                    ((imac >> 40) & 0xff),
                    ((imac >> 32) & 0xff),
                    ((imac >> 24) & 0xff),
                    ((imac >> 16) & 0xff),
                    ((imac >> 8) & 0xff),
                    ((imac >> 0) & 0xff));
                return smac;
            }
            return string.Empty;
        }

        public void GenerateSpreadsheet(string type, string MAC1st, decimal numMACs)
        {
            if (MAC1st == string.Empty || Decimal.ToInt32(numMACs) == 0)
            {
                return;
            }

            try
            {
                string[] macs = new string[Decimal.ToInt32(numMACs)];
                XmlNodeList elemList = cfgdoc.GetElementsByTagName("camera");
                int stepwidth = 0;
                SLDocument sldoc = new SLDocument();
                sldoc.SetColumnWidth("A", 25.0);

                for (int i = 0; i < elemList.Count; i++)
                {
                    if (elemList[i].Attributes["type"].Value == type)
                    {
                        stepwidth = int.Parse(elemList[i].Attributes["step"].Value);
                    }
                }

                macs[0] = MAC1st;

                for( int i = 1; i < Decimal.ToInt32(numMACs); i++)
                {
                    
                    string tmpMAC = macs[i - 1];
                    Int64 iMAC = MACStringToInt64(tmpMAC);
                    iMAC += stepwidth;
                    macs[i] = MACInt64ToString(iMAC);
                }

                for (int i = 0; i < macs.Length; i++ )
                {
                    string cell = "A" + (i + 1).ToString();
                    sldoc.SetCellValue(cell, macs[i]);
                    logger.Log(string.Format("New MAC: {0}", macs[i]), LogCategory.lcMessage);
                }

                sldoc.SaveAs(type + ".xlsx");
            }
            catch(SystemException ex)
            {
                logger.Log(ex.Message, LogCategory.lcError);
            }
        }
    }
}
