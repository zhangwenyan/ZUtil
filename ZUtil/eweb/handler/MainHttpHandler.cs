using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using eweb.ex;
using eweb.attribute;
using Newtonsoft.Json;
using System.Web.SessionState;

namespace eweb.handler
{
    public class MainHttpHandler : IHttpHandler, IRequiresSessionState
    {
        bool debug = true;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }



        private object deal(HttpContext context)
        {

            var req = context.Request;
            var url = req.Url.ToString();


            var controllerName = req.Url.AbsolutePath.Substring(1);
            controllerName = controllerName.Substring(0, controllerName.IndexOf(".")) + "Controller";
            String action = req["action"] ?? "main";
           // Type supType = asmb.GetType("EnterpriseServerBase.DataAccess.IDBAccesser");
            var conFullName = "Web.controller." + controllerName;
            var controllerType = Assembly.Load("Web").GetType(conFullName);

            if (controllerType == null)
            {
                if (debug)
                {
                    throw new MsgException("找不到类:" + conFullName);
                }

                throw new MsgException("该功能尚未实现");
            }
            var controller = Activator.CreateInstance(controllerType);

            var method = controllerType.GetMethod(action, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);




            if (method == null)
            {
                if (debug)
                {
                    throw new MsgException("找不到该方法:" + conFullName+"."+action);
                }
                throw new MsgException("该功能方法尚未实现");
            }

            var atts = method.GetCustomAttributes(typeof(Login), true);
            if (atts.Length>0)
            {
                #region 检查是否登陆

                if (context.Session["user"]==null)
                {
                    throw new MsgException("没有登录");
                }
                #endregion 检查是否登陆
            }



            var ps0 = method.GetParameters();
            var ps = new object[ps0.Length];
            for (var i = 0; i < ps.Length; i++)
            {
                var p = ps0[i];
                var type = p.ParameterType;

                if (type.Equals(typeof(HttpContext)))
                {
                    ps[i] = context;
                }
                else if (type.Equals(typeof(HttpRequest)))
                {
                    ps[i] = context.Request;
                }
                else if (type.Equals(typeof(HttpResponse)))
                {
                    ps[i] = context.Response;
                }
                else if (type.IsValueType || type.Equals(typeof(String)))
                {
                    String valueStr = req[p.Name];
                    if (valueStr != null)
                    {
                        if (valueStr == "" && !type.Equals(typeof(String)))
                        {
                            continue;
                        }

                        ps[i] = Convert.ChangeType(valueStr, type);
                    }
                }
                else
                {
                    var props = type.GetProperties();
                    var t = Activator.CreateInstance(type);
                    foreach(var prop in props)
                    {
                        if (prop.PropertyType.IsValueType || prop.PropertyType.Equals(typeof(String)))
                        {
                            String valStr = req[prop.Name];
                            if (valStr != null)
                            { 
                                if (valStr == "" && !prop.PropertyType.Equals(typeof(String)))
                                {
                                    continue;
                                }
                                prop.SetValue(t, Convert.ChangeType(valStr, prop.PropertyType), null);
                            }
                        }
                        else
                        {
                            var props1 = prop.PropertyType.GetProperties();
                            var t1 = Activator.CreateInstance(prop.PropertyType);
                            foreach (var prop1 in props1)
                            {

                                String valStr = req[prop1.Name];
                                if (valStr != null)
                                {
                                    if (valStr == "" && !prop.PropertyType.Equals(typeof(String)))
                                    {
                                        continue;
                                    }
                                    prop1.SetValue(t1, Convert.ChangeType(valStr, prop1.PropertyType), null);
                                }
                            }
                            prop.SetValue(t, t1, null);

                       }
                       
                    }
                    ps[i] = t;
                }
            }
            var result = method.Invoke(controller, ps);
            if (result == null && method.ReturnType == typeof(void))
            {
                result = new { success = true };
            }
            return result;
        }

        public void ProcessRequest(HttpContext context)
        {

            object result = null;
            try
            {
                result = deal(context);
            }
            catch(MsgException ex)
            {
                result = new
                {
                    success = false,
                    error = true,
                    msg = ex.Message
                };
            }
            catch (Exception ex)
            {
                result = new
                {
                    success = false,
                    error = true,
                    msg = "系统错误("+ex.Message+")"
                };
            }
            finally
            {
                var str = JsonConvert.SerializeObject(result);
                context.Response.ContentType = "text/json";
                context.Response.Write(str);
            }

        }


    }
}