using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Classes
{
    public class LevelGenerator
    {
        private int horizontalSize;
        private float blockEmptySpaceRatio;

        private Platform[][] level;
        
        public LevelGenerator(float blockEmptySpaceRatio, int horizontalSize)
        {
            this.blockEmptySpaceRatio = blockEmptySpaceRatio;
            this.horizontalSize = horizontalSize;
        }

        public Platform[][] Level
        {
            get
            {
                return this.level;
            }
        }

        public void GenerateInnitialLevel(int numberOfRowsToCreate)
        {
            level = new Platform[numberOfRowsToCreate][];

            for (int rowNumber = 0; rowNumber < numberOfRowsToCreate; rowNumber++)
            {
                Platform[] row = new Platform[this.horizontalSize];

                if (rowNumber == 0)
                {
                    for (int columnNumber = 0; columnNumber < this.horizontalSize; columnNumber++)
                    {
                        if (columnNumber == 0)
                        {
                            row[columnNumber] = Platform.GetRandomPlatform();
                        }
                        else if (columnNumber == this.horizontalSize - 1)
                        {
                            row[columnNumber] = row[columnNumber - 1].GetNextPlatform(row[0].LeftJoint);
                        }
                        else
                        {
                            row[columnNumber] = row[columnNumber - 1].GetNextPlatform();
                        }
                    }
                }
                else
                {
                    //TODO: Generate the rest of the rows here.
                }

                level[rowNumber] = row;
            }
        }
    }
}
