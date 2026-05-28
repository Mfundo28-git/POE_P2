using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace POE_P2
{
    public class Greeting
    {

        //void method to play the sound named greet
        public void greet()
        { 

            //replace the \bin\Debug\ from the path with greeting.wav

            string auto_path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", @"\greetings.wav");

            //create an instance for the soundPlayer class
            SoundPlayer greetMe = new SoundPlayer(auto_path);
            //then greet
            greetMe.Play();


        }//end of greet method

    }//end of class
}//end of namespace
