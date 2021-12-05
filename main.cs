using ProducerNS;
using ConsumerNS;
using CarNS;
using BufferNS;
using System;
using System.Threading;
using System.Diagnostics;

namespace Main
{
    class main
    {
        private static ManualResetEvent consumerWake = new ManualResetEvent(false);
        private static ManualResetEvent producerWake = new ManualResetEvent(false);

        static void Main(string[] args) //main
        {
            //Debug.Assert(args.Length > 0);

            //int producerAmount = Int32.Parse(args[0]);
            //int consumerAmount = Int32.Parse(args[1]);

            int producerAmount = 1000;
            int consumerAmount = 1000;

            Buffer<Car> B = new Buffer<Car>(4);

            for (int i = 0; i < producerAmount; i++)
            {/*
                create a Thread for each Producer specified and start it with its corresponding Method
            */
                Thread t = new Thread(new ParameterizedThreadStart(producerThreadMethod));
                t.Name = String.Format("Producer Thread {0}", i + 1); //name it correctly
                t.Start(B);
                Console.WriteLine(t.Name + " started");
            }

            for (int i = 0; i < consumerAmount; i++)
            {/*
                create a Thread for each Consumer specified and start it with its corresponding Method
            */
                Thread t = new Thread(new ParameterizedThreadStart(consumerThreadMethod));
                t.Name = String.Format("Consumer Thread {0}", i + 1); //name it correctly
                t.Start(B);
                Console.WriteLine(t.Name + " started");
            }
        }

        public static void producerThreadMethod(object o) //Method for the Producer threads
        {
            Debug.Assert(o != null);
            /*
            rValue states:
            0: dont put thread to sleep - normal behavior
            1: put thread to sleep - Buffer is full
            2: wake all Consumers - Buffer was empty
            */

            try
            {
                Buffer<Car> B = (Buffer<Car>)o; //Cast object to Buffer, object only gets used so threading works

                Producer p = new Producer(B); //create new Producer for each thread

                for (; ; )
                {
                    producerWake.Reset(); //reset flag in loop entry
                    switch (p.run())
                    {
                        case 0:
                            break;
                        case 1:
                            Console.WriteLine(Thread.CurrentThread.Name + " going to sleep, Buffer full");
                            producerWake.WaitOne(); //thread sleeps and code execution waits here until producerWake is signalled
                            break;
                        case 2:
                            Console.WriteLine(Thread.CurrentThread.Name + " woke all Consumers");
                            consumerWake.Set(); //wakes all consumers
                            break;
                    }
                }
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
            }
        }

        public static void consumerThreadMethod(object o) //Method for the Consumer threads
        {
            Debug.Assert(o != null);
            /*
            rValue states:
            0: dont put thread to sleep - normal bahvior
            1: put thread to sleep - Buffer is empty
            2: wake all Producers - Buffer was full
            */
            try
            {
                Buffer<Car> B = (Buffer<Car>)o; //Cast object to Buffer, object only gets used so threading works

                Consumer c = new Consumer(B); //create new Consumer for each thread

                for (; ; )
                {
                    consumerWake.Reset(); //reset flag in loop entry
                    switch (c.run())
                    {
                        case 0:
                            break;
                        case 1:
                            Console.WriteLine(Thread.CurrentThread.Name + " going to sleep, Buffer empty");
                            consumerWake.WaitOne(); //thread sleeps until consumerWake gets signalled
                            break;
                        case 2:
                            Console.WriteLine(Thread.CurrentThread.Name + " woke all Producers");
                            producerWake.Set(); //wakes all prodcuers
                            break;
                    }
                }
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}