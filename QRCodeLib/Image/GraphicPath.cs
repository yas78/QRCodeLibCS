using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ys.Image
{
    internal class GraphicPath
    {
        private enum Direction
        {
            UP = 0,
            DOWN,
            LEFT,
            RIGHT,
        }

        public static Point[][] FindContours(int[][] image)
        {
            var gpPaths = new List<Point[]>();

            for (int y = 0; y < image.Length - 1; ++y)
            {
                for (int x = 0; x < image[y].Length - 1; ++x)
                {
                    if (image[y][x] == int.MaxValue)
                        continue;

                    if (!(image[y][x] > 0 && image[y][x + 1] <= 0))
                        continue;

                    image[y][x] = int.MaxValue;
                    var start = new Point(x, y);
                    var gpPath = new List<Point> { start };

                    Direction dr = Direction.UP;
                    Point p = new Point(start.X, start.Y - 1);

                    do
                    {
                        switch (dr)
                        {
                            case Direction.UP:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y][p.X + 1] <= 0)
                                        p = new Point(p.X, p.Y - 1);
                                    else
                                    {
                                        gpPath.Add(p);
                                        dr = Direction.RIGHT;
                                        p = new Point(p.X + 1, p.Y);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X, p.Y + 1);
                                    gpPath.Add(p);
                                    dr = Direction.LEFT;
                                    p = new Point(p.X - 1, p.Y);
                                }
                                break;

                            case Direction.DOWN:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y][p.X - 1] <= 0)
                                        p = new Point(p.X, p.Y + 1);
                                    else
                                    {
                                        gpPath.Add(p);
                                        dr = Direction.LEFT;
                                        p = new Point(p.X - 1, p.Y);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X, p.Y - 1);
                                    gpPath.Add(p);
                                    dr = Direction.RIGHT;
                                    p = new Point(p.X + 1, p.Y);
                                }
                                break;

                            case Direction.LEFT:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y - 1][p.X] <= 0)
                                        p = new Point(p.X - 1, p.Y);
                                    else
                                    {
                                        gpPath.Add(p);
                                        dr = Direction.UP;
                                        p = new Point(p.X, p.Y - 1);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X + 1, p.Y);
                                    gpPath.Add(p);
                                    dr = Direction.DOWN;
                                    p = new Point(p.X, p.Y + 1);
                                }
                                break;

                            case Direction.RIGHT:
                                if (image[p.Y][p.X] > 0)
                                {
                                    image[p.Y][p.X] = int.MaxValue;

                                    if (image[p.Y + 1][p.X] <= 0)
                                        p = new Point(p.X + 1, p.Y);
                                    else
                                    {
                                        gpPath.Add(p);
                                        dr = Direction.DOWN;
                                        p = new Point(p.X, p.Y + 1);
                                    }
                                }
                                else
                                {
                                    p = new Point(p.X - 1, p.Y);
                                    gpPath.Add(p);
                                    dr = Direction.UP;
                                    p = new Point(p.X, p.Y - 1);
                                }
                                break;

                            default:
                                throw new InvalidOperationException();
                        }

                    } while (p != start);

                    gpPaths.Add(gpPath.ToArray());
                }
            }

            return gpPaths.ToArray();
        }
    }
}
