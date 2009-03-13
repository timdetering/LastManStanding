using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain.FieldOfView
{
    public class ShadowCastingFov : IFovAlgorithm
    {
        #region Fields

        //private FovShapeType fovProfile.FovShape = FovShapeType.RoundedSquare;
        private FovResultset visibleLocations = new FovResultset();

        #endregion

        #region Properties

        public FovResultset VisibleLocations
        {
            get { return visibleLocations; }
        }

        //public FovShapeType FovShape
        //{
        //    get { return fovProfile.FovShape; }
        //    set { fovProfile.FovShape = value; }
        //}

        #endregion

        #region IFovAlgorithm Members

        public FovResultset CalculateFov(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            visibleLocations = new FovResultset();
       //     fovProfile.FovShape = shapeType;

            if (terrain != null)
            {
                visibleLocations.Add(new FovObject() { Location = { Coordinate = origin }, DistanceFromOrigin = 0 });
                if (fovProfile.FovRadius > 0)
                {
                    ScanNorthwestToNorth(terrain, origin, fovProfile);
                    ScanNorthToNortheast(terrain, origin, fovProfile);
                    ScanNortheastToEast(terrain, origin, fovProfile);
                    ScanEastToSoutheast(terrain, origin, fovProfile);
                    ScanSoutheastToSouth(terrain, origin, fovProfile);
                    ScanSouthToSouthwest(terrain, origin, fovProfile);
                    ScanSouthwestToWest(terrain, origin, fovProfile);
                    ScanWestToNorthwest(terrain, origin, fovProfile);
                }
            }

            return visibleLocations;
        }

        #endregion

        #region Octant Scans

        public void ScanNorthwestToNorth(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanNorthwestToNorth(terrain, origin, fovProfile, 1, 0, 1);
        }

        private void ScanNorthwestToNorth(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                          float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var xStart = (int)Math.Floor(origin.X + 0.5 - (startSlope * distance));
            var xEnd = (int)Math.Floor(origin.X + 0.5 - (endSlope * distance));
            int yCheck = origin.Y - distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int xRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    xRadius = origin.X - (fovProfile.FovRadius / 2);
                else
                    xRadius = origin.X - CalculateRadius(fovProfile.FovRadius, distance);

                if (xStart < xRadius) xStart = xRadius;
                if (xStart > xEnd) return;
            }

            var currentLocation = new Point(xStart, yCheck);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevLocationWasBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return;

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevLocationWasBlocked)
                        ScanNorthwestToNorth(terrain, origin, fovProfile, startSlope,
                                             InverseSlope(GetCenterCoordinate(origin),
                                                          PointF.Add(currentLocation, new SizeF(-0.0000001f, 0.9999999f))),
                                             distance + 1);
                    prevLocationWasBlocked = true;
                }
                else
                {
                    if (prevLocationWasBlocked) startSlope = InverseSlope(GetCenterCoordinate(origin), currentLocation);
                    prevLocationWasBlocked = false;
                }
            }

            if (!prevLocationWasBlocked)
                ScanNorthwestToNorth(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanNorthToNortheast(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanNorthToNortheast(terrain, origin, fovProfile, 0, -1, 1);
        }

        private void ScanNorthToNortheast(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                          float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var xStart = (int)Math.Floor((origin.X + 0.5) - (startSlope * distance));
            var xEnd = (int)Math.Floor((origin.X + 0.5) - (endSlope * distance));
            int yCheck = origin.Y - distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int xRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    xRadius = origin.X + (fovProfile.FovRadius / 2);
                else
                    xRadius = origin.X + CalculateRadius(fovProfile.FovRadius, distance);

                if (xEnd > xRadius) xEnd = xRadius;
                if (xStart > xEnd) return;
            }

            var currentLocation = new Point(xStart, yCheck);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevLocationWasBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int xCheck = xStart + 1; xCheck <= xEnd; xCheck++)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return;

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevLocationWasBlocked)
                        ScanNorthToNortheast(terrain, origin, fovProfile, startSlope,
                                             InverseSlope(GetCenterCoordinate(origin),
                                                          PointF.Add(currentLocation, new SizeF(-0.0000001f, 0))),
                                             distance + 1);
                    prevLocationWasBlocked = true;
                }
                else
                {
                    if (prevLocationWasBlocked)
                        startSlope = InverseSlope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(0, 0.9999999f)));
                    prevLocationWasBlocked = false;
                }
            }

            if (!prevLocationWasBlocked)
                ScanNorthToNortheast(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanNortheastToEast(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanNortheastToEast(terrain, origin, fovProfile, -1, 0, 1);
        }

        private void ScanNortheastToEast(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                         float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var yStart = (int)Math.Floor((origin.Y + 0.5) + (startSlope * distance));
            var yEnd = (int)Math.Floor((origin.Y + 0.5) + (endSlope * distance));
            int xCheck = origin.X + distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int yRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    yRadius = origin.Y - (fovProfile.FovRadius / 2);
                else
                    yRadius = origin.Y - CalculateRadius(fovProfile.FovRadius, distance);

                if (yStart < yRadius) yStart = yRadius;
                if (yStart > yEnd) return;
            }

            var currentLocation = new Point(xCheck, yStart);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return;

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevBlocked)
                        ScanNortheastToEast(terrain, origin, fovProfile, startSlope,
                                            Slope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(0, -0.0000001f))), distance + 1);
                    prevBlocked = true;
                }
                else
                {
                    if (prevBlocked)
                        startSlope = Slope(GetCenterCoordinate(origin),
                                           PointF.Add(currentLocation, new SizeF(0.9999999f, 0)));
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) ScanNortheastToEast(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanEastToSoutheast(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanEastToSoutheast(terrain, origin, fovProfile, 0, 1, 1);
        }

        private void ScanEastToSoutheast(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                         float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var yStart = (int)Math.Floor((origin.Y + 0.5) + (startSlope * distance));
            var yEnd = (int)Math.Floor((origin.Y + 0.5) + (endSlope * distance));
            int xCheck = origin.X + distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int yRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    yRadius = origin.Y + (fovProfile.FovRadius / 2);
                else
                    yRadius = origin.Y + CalculateRadius(fovProfile.FovRadius, distance);

                if (yEnd > yRadius) yEnd = yRadius;
                if (yStart > yEnd) return;
            }

            var currentLocation = new Point(xCheck, yStart);
            if (!terrain.Bounds.Contains(currentLocation))
                return; 
            
            SetAsVisible(terrain, currentLocation, distance);
            bool prevBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int yCheck = yStart + 1; yCheck <= yEnd; yCheck++)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return; 


                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevBlocked)
                        ScanEastToSoutheast(terrain, origin, fovProfile, startSlope,
                                            Slope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(1, 0))), distance + 1);
                    prevBlocked = true;
                }
                else
                {
                    if (prevBlocked) startSlope = Slope(GetCenterCoordinate(origin), currentLocation);
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) ScanEastToSoutheast(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanSoutheastToSouth(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanSoutheastToSouth(terrain, origin, fovProfile, 1, 0, 1);
        }

        private void ScanSoutheastToSouth(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                          float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var xStart = (int)Math.Floor((origin.X + 0.5) + (startSlope * distance));
            var xEnd = (int)Math.Floor((origin.X + 0.5) + (endSlope * distance));
            int yCheck = origin.Y + distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int xRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    xRadius = origin.X + (fovProfile.FovRadius / 2);
                else
                    xRadius = origin.X + CalculateRadius(fovProfile.FovRadius, distance);

                if (xStart > xRadius) xStart = xRadius;
                if (xStart < xEnd) return;
            }

            var currentLocation = new Point(xStart, yCheck);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevLocationWasBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return; 

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevLocationWasBlocked)
                        ScanSoutheastToSouth(terrain, origin, fovProfile, startSlope,
                                             InverseSlope(GetCenterCoordinate(origin),
                                                          PointF.Add(currentLocation, new SizeF(1, 0))), distance + 1);
                    prevLocationWasBlocked = true;
                }
                else
                {
                    if (prevLocationWasBlocked)
                        startSlope = InverseSlope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(0.9999999f, 0.9999999f)));
                    prevLocationWasBlocked = false;
                }
            }

            if (!prevLocationWasBlocked)
                ScanSoutheastToSouth(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanSouthToSouthwest(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanSouthToSouthwest(terrain, origin, fovProfile, 0, -1, 1);
        }

        private void ScanSouthToSouthwest(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                          float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var xStart = (int)Math.Floor(origin.X + 0.5 + (startSlope * distance));
            var xEnd = (int)Math.Floor(origin.X + 0.5 + (endSlope * distance));
            int yCheck = origin.Y + distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int xRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    xRadius = origin.X - (fovProfile.FovRadius / 2);
                else
                    xRadius = origin.X - CalculateRadius(fovProfile.FovRadius, distance);

                if (xEnd < xRadius) xEnd = xRadius;
                if (xStart < xEnd) return;
            }

            var currentLocation = new Point(xStart, yCheck);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevLocationWasBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int xCheck = xStart - 1; xCheck >= xEnd; xCheck--)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return; 

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevLocationWasBlocked)
                        ScanSouthToSouthwest(terrain, origin, fovProfile, startSlope,
                                             InverseSlope(GetCenterCoordinate(origin),
                                                          PointF.Add(currentLocation, new SizeF(1, 1))), distance + 1);
                    prevLocationWasBlocked = true;
                }
                else
                {
                    if (prevLocationWasBlocked)
                        startSlope = InverseSlope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(0.9999999f, 0)));
                    prevLocationWasBlocked = false;
                }
            }

            if (!prevLocationWasBlocked)
                ScanSouthToSouthwest(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanSouthwestToWest(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanSouthwestToWest(terrain, origin, fovProfile, -1, 0, 1);
        }

        private void ScanSouthwestToWest(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                         float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var yStart = (int)Math.Floor((origin.Y + 0.5) - (startSlope * distance));
            var yEnd = (int)Math.Floor((origin.Y + 0.5) - (endSlope * distance));
            int xCheck = origin.X - distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int yRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    yRadius = origin.Y + (fovProfile.FovRadius / 2);
                else
                    yRadius = origin.Y + CalculateRadius(fovProfile.FovRadius, distance);

                if (yStart > yRadius) yStart = yRadius;
                if (yStart < yEnd) return;
            }

            var currentLocation = new Point(xCheck, yStart);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return; 

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevBlocked)
                        ScanSouthwestToWest(terrain, origin, fovProfile, startSlope,
                                            Slope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(1, 1))), distance + 1);
                    prevBlocked = true;
                }
                else
                {
                    if (prevBlocked)
                        startSlope = Slope(GetCenterCoordinate(origin),
                                           PointF.Add(currentLocation, new SizeF(0, 0.9999999f)));
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) ScanSouthwestToWest(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        public void ScanWestToNorthwest(TerrainMap terrain, Point origin, IFovProfile fovProfile)
        {
            ScanWestToNorthwest(terrain, origin, fovProfile, 0, 1, 1);
        }

        private void ScanWestToNorthwest(TerrainMap terrain, Point origin, IFovProfile fovProfile, float startSlope,
                                         float endSlope, int distance)
        {
            if (distance > fovProfile.FovRadius) return;

            var yStart = (int)Math.Floor((origin.Y + 0.5) - (startSlope * distance));
            var yEnd = (int)Math.Floor((origin.Y + 0.5) - (endSlope * distance));
            int xCheck = origin.X - distance;

            if ((fovProfile.FovShape == FovShapeType.Circle) || (fovProfile.FovShape == FovShapeType.RoundedSquare))
            {
                int yRadius;
                if ((fovProfile.FovShape == FovShapeType.RoundedSquare) && (distance == fovProfile.FovRadius))
                    yRadius = origin.Y - (fovProfile.FovRadius / 2);
                else
                    yRadius = origin.Y - CalculateRadius(fovProfile.FovRadius, distance);

                if (yEnd < yRadius) yEnd = yRadius;
                if (yStart < yEnd) return;
            }

            var currentLocation = new Point(xCheck, yStart);
            if (!terrain.Bounds.Contains(currentLocation))
                return;

            SetAsVisible(terrain, currentLocation, distance);
            bool prevBlocked = fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]);

            for (int yCheck = yStart - 1; yCheck >= yEnd; yCheck--)
            {
                currentLocation = new Point(xCheck, yCheck);
                if (!terrain.Bounds.Contains(currentLocation))
                    return; 

                SetAsVisible(terrain, currentLocation, distance);

                if (fovProfile.LosIsBlockedByTerrain(terrain[currentLocation]))
                {
                    if (!prevBlocked)
                        ScanWestToNorthwest(terrain, origin, fovProfile, startSlope,
                                            Slope(GetCenterCoordinate(origin),
                                                  PointF.Add(currentLocation, new SizeF(0, 1))), distance + 1);
                    prevBlocked = true;
                }
                else
                {
                    if (prevBlocked)
                        startSlope = Slope(GetCenterCoordinate(origin),
                                           PointF.Add(currentLocation, new SizeF(0.9999999f, 0.9999999f)));
                    prevBlocked = false;
                }
            }

            if (!prevBlocked) ScanWestToNorthwest(terrain, origin, fovProfile, startSlope, endSlope, distance + 1);
        }

        #endregion

        #region Helper Methods

        private static int CalculateRadius(int circleRadius, int positionOnAxis)
        {
            return (int)Math.Round(Math.Sqrt((circleRadius * circleRadius) - (positionOnAxis * positionOnAxis)), 0);
        }

        private void SetAsVisible(TerrainMap terrain, Point location, int distance)
        {
            if ((terrain.Bounds.Contains(location)) && (!visibleLocations.ContainsLocation(location)))
                visibleLocations.Add(new FovObject() { Location = new Location() { Coordinate = location }, DistanceFromOrigin = distance });
        }

        private static PointF GetCenterCoordinate(PointF location)
        {
            return PointF.Add(location, new SizeF(0.5f, 0.5f));
        }

        private static float Slope(PointF source, PointF destination)
        {
            float deltaX = source.X - destination.X;
            float deltaY = source.Y - destination.Y;

            if (deltaX != 0.0f) return deltaY / deltaX;

            return 0f;
        }

        private static float InverseSlope(PointF source, PointF destination)
        {
            float slope = Slope(source, destination);

            if (slope != 0.0f) return 1 / slope;

            return 0f;
        }

        #endregion
    }
}
