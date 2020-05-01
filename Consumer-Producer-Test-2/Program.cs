using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace Consumer_Producer_Test_2
{
  public class DataflowProducerConsumer
  {
    public static void Main(string[] args)
    {
      PC pc = new PC();

      Thread t1 = new Thread(new ThreadStart(() => {
        try {
          pc.Produce();
        } catch(Exception) {
          throw;
        }
      }));

      Thread t2 = new Thread(new ThreadStart(() => {
        try {
          pc.Consume();
        } catch(Exception) {
          throw;
        }
      }));

      t1.Start();
      t2.Start();

      t1.Join();
      t2.Join();
    }

    public class PC
    {
      Random rand = new Random();
      List<char> list = new List<char>();

      int beg = 0, end = 0, capacity = 30;

      public void PrintAll()
      {
        string output = "";
        for(int i = 0; i < capacity; ++i) {
          output += list[i] == '_' ? "_" : list[i].ToString();
          output += " ";
        }
        Console.Out.WriteLine(output);
      }

      public void Produce()
      {
        //int value = 1;
        list.AddRange(Enumerable.Repeat('_', capacity));

        while(true) {
          lock(this) {
            Thread.Sleep(1000);

            int temp = rand.Next(0, 100);
            if(temp < 60) {
              Thread.Sleep(500);
              temp = rand.Next(3, 10);
              for(int i = 3; i < temp; i++) {
                // producer thread waits while list is full 
                while((beg + 1) % capacity == end) Monitor.Wait(this);

                list[beg] = '*';
                beg = (beg + 1) % capacity;

                Monitor.Pulse(this);
              }

              PrintAll();
            }
          }
        }
      }

      public void Consume()
      {
        while(true) {
          lock(this) {
            Thread.Sleep(1000);

            int temp = rand.Next(0, 100);
            if(temp < 60) {
              Thread.Sleep(500);
              temp = rand.Next(3, 10);
              for(int i = 3; i < temp; i++) {
                // consumer thread waits while list is empty 
                while(beg == end) Monitor.Wait(this);

                list[end] = '_';
                end = (end + 1) % capacity;

                Monitor.Pulse(this);
              }
            }
          }
        }
      }

    }
  }
}
