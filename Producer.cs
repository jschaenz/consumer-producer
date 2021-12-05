using BufferNS;
using CarNS;
using System;
using System.Threading;
using System.Diagnostics;

namespace ProducerNS
{
    public class Producer
    {
        private Buffer<Car> B;
        private readonly Random rand = new Random();
        public Producer(Buffer<Car> buffer) //constructor
        {
            this.B = buffer;
        }

        public int run() //implements functionality of Producer
        {
            Debug.Assert(B != null);

            /*
            rValue states:
            0: dont put thread to sleep - normal behavior
            1: put thread to sleep - Buffer is full
            2: wake all Consumers - Buffer was empty
            */

            int rVal = 0;

            int waitTime = rand.Next(10000);
            Thread.CurrentThread.Join(waitTime); //randomly let thread wait

            if (!B.full())
            {
                if (B.empty()) //if Buffer is empty, wake up all consumers
                {
                    rVal = 2; //wake up consumers
                }

                B.push(new Car()); //insert new Car in Buffer
                Console.WriteLine(Thread.CurrentThread.Name + " added Car to Buffer");
            }
            else
            {
                rVal = 1; // put thread to sleep
            }

            Debug.Assert(!B.empty());
            Debug.Assert(rVal == 0 || rVal == 1 || rVal == 2);
            return rVal;
        }
    }
}