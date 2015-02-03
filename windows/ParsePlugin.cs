/*
Copyright (c) 2015 Sky Mirror Technology Co., Ltd. <https://www.skymirror.com.tw>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

 */

using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using WPCordovaClassLib.Cordova.JSON;

namespace WPCordovaClassLib.Cordova.Commands
{
    class ParsePlugin : BaseCommand
    {
        static Boolean hasRegisteredEvent = false;

        public static string GetAppName()
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = new XmlXapResolver()
            };

            using (var xmlReader = XmlReader.Create("WMAppManifest.xml", xmlReaderSettings))
            {
                xmlReader.ReadToDescendant("App");

                return xmlReader.GetAttribute("Title");
            }
        }
        private void registerEvent()
        {

            var nameHelper = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            
            hasRegisteredEvent = true;
            ParsePush.ToastNotificationReceived += (sender, args) =>
            {
                var json = ParsePush.PushJson(args);
                object alertMessage;
                if (json.TryGetValue("alert", out alertMessage))
                {
                    // Display a dialog of all the fields in the toast.
                    Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(alertMessage.ToString(), GetAppName(), MessageBoxButton.OK));
                }
            };
        }
        // register getInstallationId getInstallationObjectId getSubscriptions subscribe unsubscribe
        public void register(string notused)
        { 
        }

        public void getInstallationId(string notused)
        {
            var a = ParseInstallation.CurrentInstallation.InstallationId.ToString();

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, ParseInstallation.CurrentInstallation.InstallationId.ToString())); ;

        }
        
        public void getInstallationObjectId(string notused)
        {
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, ParseInstallation.CurrentInstallation.ObjectId.ToString())); ;
        }

        public void getSubscriptions(string notused)
        {
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, ParseInstallation.CurrentInstallation.Channels.ToString())); ;
        }


        public async void subscribe(string arg)
        {
            string channel = JsonHelper.Deserialize<string[]>(arg)[0];
            await ParsePush.SubscribeAsync(channel);

            if (hasRegisteredEvent == false)
                registerEvent();

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, ""));
        }

        public async void unsubscribe(string arg)
        {
            string channel = JsonHelper.Deserialize<string[]>(arg)[0];
            await ParsePush.UnsubscribeAsync(channel);

           DispatchCommandResult(new PluginResult(PluginResult.Status.OK, ""));
        }
    }
}
