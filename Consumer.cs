using BufferNS;
using CarNS;
using System;
using System.Threading;
using System.Diagnostics;

namespace ConsumerNS
{
    public class Consumer
    {
        private Buffer<Car> B;
        private readonly Random rand = new Random();
        public Consumer(Buffer<Car> buffer)
        {
            this.B = buffer;
        }

        public int run() //implements functionality of Consumer
        {
            Debug.Assert(B != null);
            /*
           rValue states:
           0: dont put thread to sleep - normal bahvior
           1: put thread to sleep - Buffer is empty
           2: wake all Producers - Buffer was full
           */

            int rVal = 0;

            int waitTime = rand.Next(10000);
            Thread.CurrentThread.Join(waitTime); //randomly let thread wait

            if (!B.empty())
            {
                if (B.full()) //if Buffer is full, wake up all Producers
                {
                    rVal = 2; //wake up producers
                }

                B.pop(); //remove car
                Console.WriteLine(Thread.CurrentThread.Name + " removed Car from Buffer");
            }
            else
            {
                rVal = 1; //put thread to sleep
            }

            Debug.Assert(!B.full());
            Debug.Assert(rVal == 0 || rVal == 1 || rVal == 2);
            return rVal;
        }
    }
}