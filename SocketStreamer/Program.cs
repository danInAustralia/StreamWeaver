using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketStreamer
{
    public class MicPlayer
    {
        private BufferedWaveProvider bwp;
        private TcpClient clientSocket;

        public void MicPlayback()
        {
            WaveFormat format = new WaveFormat(48000, 16, 1);
            bwp = new BufferedWaveProvider(format);
            WaveIn wi;
            WaveOut waveOut;

            try
            {
                //listenter.Bind(localEndPoint);
                //listenter.Listen(10);
                //Socket handler = listenter.Accept();
                bwp.DiscardOnBufferOverflow = true;


                    //using (IWavePlayer waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 24))
                    {
                        //while (true)
                        {
                            waveOut = new WaveOut();
                            int numberOfDevices = WaveIn.DeviceCount;
                            wi = new WaveIn();
                            wi.DeviceNumber = 0;
                            wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);
                            waveOut.Init(bwp);
                            wi.WaveFormat = format;
                            wi.StartRecording();
                            waveOut.Play();
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void MicToServer(string serverAddress, int serverPort)
        {
            IPAddress ipAddress = IPAddress.Parse(serverAddress);
            clientSocket = new TcpClient();
            clientSocket.Connect(ipAddress, serverPort);
            WaveFormat format = new WaveFormat(48000, 16, 1);
            bwp = new BufferedWaveProvider(format);
            WaveIn wi;
            //WaveOut waveOut;


            try
            {
                //listenter.Bind(localEndPoint);
                //listenter.Listen(10);
                //Socket handler = listenter.Accept();
                bwp.DiscardOnBufferOverflow = true;


                //using (IWavePlayer waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 24))
                {
                    //while (true)
                    {
                        //waveOut = new WaveOut();
                        int numberOfDevices = WaveIn.DeviceCount;
                        wi = new WaveIn();
                        wi.WaveFormat = format;
                        wi.DeviceNumber = 0;
                        wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailableToStream);
                        //waveOut.Init(bwp);
                        wi.StartRecording();
                        //waveOut.Play();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //public void RecordCardOutputToFile()
        //{
        //    WaveFormat format = new WaveFormat(48000, 16, 1);
        //    bwp = new BufferedWaveProvider(format);
        //    WaveIn wi;
        //    WaveOut waveOut;

        //    try
        //    {
        //        //listenter.Bind(localEndPoint);
        //        //listenter.Listen(10);
        //        //Socket handler = listenter.Accept();
        //        bwp.DiscardOnBufferOverflow = true;


        //        //using (IWavePlayer waveOut = new WasapiOut(NAudio.CoreAudioApi.AudioClientShareMode.Shared, false, 24))
        //        {
        //            //while (true)
        //            {
        //                waveOut = new WasapiLoopbackCapture();
        //                int numberOfDevices = WaveIn.DeviceCount;
        //                wi = new WaveIn();
        //                wi.DeviceNumber = 0;
        //                wi.DataAvailable += new EventHandler<WaveInEventArgs>(wi_DataAvailable);
        //                waveOut.Init(bwp);
        //                wi.WaveFormat = format;
        //                wi.StartRecording();
        //                waveOut.Play();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}

        /// <summary>
        /// read samples off the network and play them on the sound card
        /// </summary>
        /// <param name="ipAddressS"></param>
        /// <param name="port"></param>
        public void NetworkPlaybackServer(string ipAddressS, int port)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                WaveFormat format = new WaveFormat(48000, 16, 1);
                bwp = new BufferedWaveProvider(format);
                bwp.DiscardOnBufferOverflow = true;
                byte[] bytes = new Byte[1024];
                int bytesRec;

                WaveOut waveOut = new WaveOut();
                waveOut.Init(bwp);
                waveOut.Play();

                IPAddress ipAddress = IPAddress.Parse(ipAddressS);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                //Socket listenter = new Socket(AddressFamily.InterNetwork,
                //                        SocketType.Stream,
                //                        ProtocolType.Tcp);
                UdpClient listener = new UdpClient(localEndPoint);

                //listenter.Bind(localEndPoint);
                //listenter.Listen(10);
                //Socket handler = listenter.Accept();

                int count = 0;
                //play back network stream until stop is sent.

                while (true)
                {
                    bytes = listener.Receive(ref localEndPoint);//handler.Receive(bytes);
                    bwp.AddSamples(bytes, 0, bytes.Length);
                    if (bytes.ToString().Contains("<STOP>"))
                    {
                        break;
                    }
                    if (waveOut.PlaybackState != PlaybackState.Playing)
                    {
                        waveOut.Play();
                    }
                    count++;
                }
                //handler.Close();
                listener.Close();
                waveOut.Stop();
            };
            worker.RunWorkerAsync();
        }

        void wi_DataAvailable(object sender, WaveInEventArgs e)
        {
            bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);

        }

        void wi_DataAvailableToStream(object sender, WaveInEventArgs e)
        {
            NetworkStream serverStream = clientSocket.GetStream();
            serverStream.Write(e.Buffer, 0, e.BytesRecorded);
            serverStream.Flush();
            //bwp.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //Socket listenter = new Socket(AddressFamily.InterNetwork,
            //                        SocketType.Stream,
            //                        ProtocolType.Tcp);
            MicPlayer player = new MicPlayer();
            player.MicPlayback();



        }
    }
}
