using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapManagement
{
    /// <summary>
    /// AStart path finding algorithm
    /// </summary>
    public class PathFinder<T> where T : class, IMapElement
    {
        /// <summary>
        /// Searches for shortest way from <paramref name="inElement"/> to <paramref name="outElement"/>
        /// </summary>
        /// <param name="inElement">Point where path begins</param>
        /// <param name="outElement">Point where path ends</param>
        /// <param name="grid">Two dimentional array of field elements</param>
        /// <returns>Shortest path, or empty array if there is no such one</returns>
        public List<T> FindPath(T inElement, T outElement, T[,] grid)
        {
            T target = inElement;
            int size = grid.GetLength(0);
            int stopper = 0;
            //Списки
            List<T> openList = new List<T>();
            List<T> closeList = new List<T>();

            openList.Add(inElement);//Добавляем в открытый список стартовую клетку
            inElement.List = 1;
            grid[inElement.X, inElement.Y].List = 1;

            while (!target.Equals(outElement) && stopper < size * size
                //Проверка размера закрытого списка если обработано слишком много клеток
                && openList.Count > 0 && closeList.Count < size * 5)
            {
                //Пока не дойдем до конца или открытый список не опустеет :
                stopper++;
                //Обновляем значения в списках
                //Refresh(OpenList);
                //Refresh(CloseList);
                //Если пока находимся в закрытом, то заменяем
                target = openList[0];
                //Идем по открытому списку
                foreach (T cl in openList)
                {
                    //Если минимальное значение F, то заменяем
                    if (target.FValue > cl.FValue)
                    {
                        target = grid[cl.X, cl.Y];
                    }
                }
                //Находим клетку из открытого списка с минимальным значением F и перемещаем в закрытый
                openList.Remove(target);
                closeList.Add(target);
                target.List = 2;
                grid[target.X, target.Y].List = 2;

                for (int i = 1; i >= -1; i--)
                {
                    for (int j = 1; j >= -1; j--)
                    {
                        if (/*(i == 0 && j != 0) || (i != 0 && j == 0)*///Без диагоналей
                            i != 0 || j != 0)
                        {
                            //Проверяем все соседние для активной клетки
                            if (target.X + i >= size || target.X + i < 0
                            || target.Y + j >= size || target.Y + j < 0)
                            {
                                //Вышли за пределы массива
                            }
                            else
                            {
                                //Игнорируем занятые клетки или находящиеся в закрытом списке
                                if (grid[target.X + i, target.Y + j].Free
                                && grid[target.X + i, target.Y + j].List != 2)//Через переменную
                                {
                                    //Клетка уже в открытом списке
                                    if (grid[target.X + i, target.Y + j].List == 1)//Через переменную
                                    {
                                        //Проверяем не короче ли путь через эту клетку
                                        if (grid[target.X + i, target.Y + j].GValue
                                        > target.GValue + 10)
                                        {
                                            //Если короче, то меняем ее родителя на текущую и ...
                                            grid[target.X + i, target.Y + j].ParentX = target.X;
                                            grid[target.X + i, target.Y + j].ParentZ = target.Y;
                                            //Пересчитываем значения
                                            int zero = i * j;
                                            FindValue(grid[target.X + i, target.Y + j], target, outElement, zero);
                                        }
                                    }
                                    else
                                    {//Клетка не в списке
                                     //Добавляем в список
                                        openList.Add(grid[target.X + i, target.Y + j]);
                                        grid[target.X + i, target.Y + j].List = 1;

                                        //Высчитываем значение F
                                        int zero = i * j;
                                        FindValue(grid[target.X + i, target.Y + j], target, outElement, zero);
                                        //Делаем текущую активную ее родителем
                                        grid[target.X + i, target.Y + j].ParentX = target.X;
                                        grid[target.X + i, target.Y + j].ParentZ = target.Y;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (stopper >= size * size || openList.Count < 1 || closeList.Count >= size * 5)
            {
                //Тупик
                CleanList(closeList);
                CleanList(openList);
                return new List<T>();
            }
            //Нашли путь или оказались в тупике

            List<T> path = new List<T>();
            int m = 0;

            outElement = grid[outElement.X, outElement.Y];

            path.Add(outElement);
            while (!inElement.Equals(outElement) && m < size)
            {
                m++;

                outElement = grid[outElement.ParentX, outElement.ParentZ];
                path.Add(outElement);
            }

            path.Reverse();

            CleanList(closeList);
            CleanList(openList);

            return path;
        }

        private void CleanList(IEnumerable<T> list)
        {
            foreach (T element in list)
            {
                element.List = 0;
                element.FValue = 0;
            }
        }

        public void FindValue(T element, T inT, T outT)
        {
            element.HValue = (Mathf.Abs(outT.X - element.X) + Mathf.Abs(outT.Y - element.Y)) * 10;
            element.GValue = inT.GValue + 10;
            element.FValue = element.HValue + element.GValue;
        }

        public void FindValue(T element, T inT, T outT, int zero)
        {
            element.HValue = (Mathf.Abs(outT.X - element.X) + Mathf.Abs(outT.Y - element.Y)) * 10;
            if (zero == 0)
            {
                element.GValue = inT.GValue + 10; //straight
            }
            else
            {
                element.GValue = inT.GValue + 14; //diagonally
            }
            element.FValue = element.HValue + element.GValue;
        }
    }
}