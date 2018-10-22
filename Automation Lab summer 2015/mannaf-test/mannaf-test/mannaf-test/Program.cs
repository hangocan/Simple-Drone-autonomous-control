using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;


namespace mannaf_test
{
    public partial class Program
    {
        Gadgeteer.Networking.WebEvent takePicture;
        GT.Picture pic = null;
        void ProgramStarted()
        {
            // ethernetJ11D.UseDHCP();
            Debug.Print("Program Started");

            ethernetJ11D.UseStaticIP("192.168.1.2", "255.255.255.0", "192.168.1.1");

            ethernetJ11D.NetworkUp += new GTM.Module.NetworkModule.NetworkEventHandler(ethernet_NetworkUp);

            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            pic = picture;
            displayTE35.SimpleGraphics.Clear();
            displayTE35.SimpleGraphics.DisplayImage(picture, 0, 0);
            resp.Respond(picture);
        }

        void ethernet_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            string ipaddress = ethernetJ11D.NetworkSettings.IPAddress;
            Debug.Print("IP:" + ipaddress.ToString());
            Gadgeteer.Networking.WebServer.StartLocalServer(ipaddress, 80);
            //foreach (string s in sender.NetworkSettings.DnsAddresses)
            //    Debug.Print("Dns:" + s);
            
            takePicture = Gadgeteer.Networking.WebServer.SetupWebEvent("takepicture");
            takePicture.WebEventReceived += new WebEvent.ReceivedWebEventHandler(takePicture_WebEventReceived);
                    }

        Responder resp = null;
        void takePicture_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            resp = responder;
            camera.TakePicture();
        }


    }
}
