using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    }
}
