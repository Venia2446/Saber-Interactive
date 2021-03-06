
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerialDeserial
{
    class ListNode
    {
        public ListNode Prev;
        public ListNode Next;
        public ListNode Rand; 
        public string Data;
    }

    class ListRand
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Serialize(FileStream s)
        {

            StringBuilder _stringOut = new StringBuilder();
            ListNode[] _nodeList = new ListNode[Count];
            _nodeList[0] = Head; // задаём Head вручную тк это позволит нам в дальнейшем сформировать список
            for (int i = 1; i < Count; i++) // в цикле проходимся по элементам коллекции и в следующий элемент i записываем то 
                                            //что хранится в Next у i-1 элемента
            {
                ListNode currentNode = _nodeList[i - 1].Next; 
                _nodeList[i] = currentNode;
            }
            foreach (var _nods in _nodeList) // заполняем новый массив, в котором хранятся Data каждого экземпляра класса ListNode;
                                             // В дальнейшем сопоставление Data и соответствующего экземпляра класса
                                             // происходит по индексу хранения Data в списке _nodsData
            {
                string _nodsData = _nods.Data;
                _stringOut.Append(_nodsData + '\0'); // символ \0 будет в дальнейшем выступать в роли сепаратора
            }

            Dictionary<ListNode, int> RandDict = new Dictionary<ListNode, int>(Count); // создаём словарь для хранения индексов экземпляров класса 
                                                                                       // из коллекции _nodeList;
                                                                                       // словарь нужен для того чтобы обеспечить сложность
                                                                                       // алгоритма поиска индексов для Rand < n^2                                                                                   
            
            
            for (int i = 0; i < Count; i++)
            {
                RandDict.Add(_nodeList[i], i); // инициализируем словарь
                                               // (Key: экземпляр класса, Value: индекс этого экземпляра в _nodeList)
            }

            foreach (var item in _nodeList) // ищем в словаре вхождения Rand экземпляра и записываем индекс вхождения в конец _stringOut
            {     
                var ans = RandDict[item.Rand];
                _stringOut.Append(ans);
            }

            // дальнейшая часть кода преобразует _stringOut в байты (сериализиует) и записывает в файл.

            string _stringOutToString = _stringOut.ToString();
            byte[] array = Encoding.Default.GetBytes(_stringOutToString); 
            s.Write(array, 0, array.Length); 
        }

        public void Deserialize(FileStream s)
        {
            // преобразуем обратно данные из байтов в строку

            byte[] _outStringToByte = new byte[s.Length]; 
            s.Read(_outStringToByte, 0, _outStringToByte.Length); 
            string _decompilText = Encoding.Default.GetString(_outStringToByte); 
            
            
            string[] _dataList = _decompilText.Split(new char[] { '\0' }); // разделяем строку по сепаратору 


            Count = _dataList.Length-1;  // ипользуем как поле Сount в классе ListRand;
                                         // Length-1 тк последний элемент массива используется для храннения ссылок Rand
                                         // на индексы экземпляров

            

            int[] RandIndexList = _dataList[Count].Select(f => (int)char.GetNumericValue(f)).ToArray(); // преобразуем последнюю строку _datalist
                                                                                                        //  содержащую ссылки Rand на индексы
                                                                                                        //  соответствующих экземпляров int массив



            ListNode[] _decompiListNode = new ListNode[Count]; // массив для хранения десериализуемых экземпляров класса;
                                                               // так же по индексам хранения будем инициализировать поля 

            for (int i = 0; i < Count; i++) // инициализируем массив _decompiListNode декомпилируемыми экземплярами класса
            {
                _decompiListNode[i] = new ListNode(); 
            } 

            for (int i = 0; i < Count; i++) // Уже в заполненном списке ListNode инициализируем поля Head,Tail;
                                            // для соответствующих экземпляров ListNode инициализируем поля Next,Prev,Rand)

            {
                _decompiListNode[i].Data = _dataList[i];

                if (i == 0) // условия для инициализации Head и Prev
                {
                    _decompiListNode[i].Prev = null; // первый экземпляр в списке не может иметь Prev 
                    Head = _decompiListNode[i]; 
                }
                else
                {
                    _decompiListNode[i].Prev = _decompiListNode[i - 1];
                }


                if (i == Count - 1) //  условия для инициализации Tail и Next
                {
                    _decompiListNode[i].Next = null; // последний экземпляр в списке не может иметь Next 
                    Tail = _decompiListNode[i];
                }
                else 
                {
                    _decompiListNode[i].Next = _decompiListNode[i + 1];
                }

                _decompiListNode[i].Rand = _decompiListNode[RandIndexList[i]];// Инициализация Rand - сопоставялем индексы Rand
                                                                              // на соотвествующие экземпляры классов
                                                                              // из списка RandIndexList c индеками массивов из __decompiListNode
                                                                              // (тк у нас восстановлен порядок экземпляров классов)
            }
        }
    }
}