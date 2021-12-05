using System;
using System.Threading;
using System.Diagnostics;

namespace BufferNS
{
    public class Buffer<T>
    {
        private class Node
        {
            /* https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/  14.12
            
                Inner Node Class to make generic Buffer work
             */
            public Node(T t) //node Constructor
            {
                next = null;
                data = t;
            }

            private Node next;
            public Node Next //next Node constructor
            {
                get { return next; } //if its used to access the next Node, next gets returned
                set { next = value; } //if its used to set the next Node, it gets saved in next
            }

            // T as private member data type.
            private T data;

            // T as return type of property.
            public T Data
            {
                get { return data; } //if its used to get Data, it returns previously set Data 
                set { data = value; } //if its used to set Data, the Data gets saved in data
            }
        }

        private Node head; //starting Node
        private int maxSize;
        private static Mutex mut = new Mutex();

        public Buffer(int maxsize) //constructor
        {
            this.maxSize = maxsize;
            this.head = null;
        }

        public void push(T e) //tries to insert e into the Buffer using exclusive access
        {
            Debug.Assert(e != null);
            Debug.Assert(!full());

            if (mut.WaitOne(1000)) //if it cant access mutex within 1 second, doesnt enter 
            {
                Console.WriteLine(Thread.CurrentThread.Name + " Mutex access");

                Node n = new Node(e);
                n.Next = head;
                head = n;
                //creates new node and inserts it at the beginning

                mut.ReleaseMutex();
                //releases its exclusive access over the mutex
                Console.WriteLine(Thread.CurrentThread.Name + " Mutex release");
            }
            else
            {
                Console.WriteLine(Thread.CurrentThread.Name + "couldnt Access the Mutex");
            }
            //Debug.Assert(!empty()); //can technically be empty if the mutex couldnt be accessed
        }

        public void pop() //removes an Element from the Buffer
        {
            Debug.Assert(!empty());

            if (mut.WaitOne(1000)) //if it cant access mutex within 1 second, doesnt enter 
            {
                Console.WriteLine(Thread.CurrentThread.Name + " Mutex access");

                head = head.Next; //removes head from the Buffer

                mut.ReleaseMutex();
                //releases its exclusive access over the mutex
                Console.WriteLine(Thread.CurrentThread.Name + " Mutex release");
            }
            else
            {
                Console.WriteLine(Thread.CurrentThread.Name + "couldnt Acces the Mutex");
            }
        }

        public bool full() //returns true if the Buffer is full
        {
            int size = 0;
            bool rVal;
            Node n = head;

            while (n != null) //loop through Buffer and count Nodes
            {
                size++;
                n = n.Next;
            }

            if (size >= this.maxSize)
            {
                rVal = true;
            }
            else
            {
                rVal = false;
            }
            return rVal;
        }


        public bool empty() //return true if Buffer is empty
        {
            bool rVal;
            if (head == null) //if the first element doesnt exist, it has to be empty
            {
                rVal = true;
            }
            else
            {
                rVal = false;
            }
            return rVal;
        }
    }
}