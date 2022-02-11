using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace AR_Rules
{
    public static class Core
    {
        public static IList<Element> GetData(Document document, ElementType type, ElementValue[] filters = null)
        {
            filters = filters ?? new ElementValue[] { ElementValue.NOT_PLACED, ElementValue.NOT_ENCLOSED, ElementValue.REDUNDANT };

            IList<Element> elements = GetElements(document, type);

            for (int i = elements.Count - 1; i >= 0; i--)
            {
                SpatialElement element = (SpatialElement)elements[i];

                ElementValue value = Distinguish(element);

                if (!filters.Contains(value))
                {
                    elements.Remove(element);
                }
            }

            return elements;
        }

        private static IList<Element> GetElements(Document document, ElementType type)
        {
            FilteredElementCollector collector = new FilteredElementCollector(document);

            if (type == ElementType.AREA)
            {
                collector.WherePasses(new AreaFilter());
            }
            else
            {
                collector.WherePasses(new RoomFilter());
            }

            IList<Element> elements = collector.ToElements();

            return elements;
        }

        private static ElementValue Distinguish(SpatialElement element)
        {
            if (element.Area > 0)
            {
                return ElementValue.PLACED;
            }
            else if (element.Location == null)
            {
                return ElementValue.NOT_PLACED;
            }
            else
            {
                SpatialElementBoundaryOptions opt = new SpatialElementBoundaryOptions();

                IList<IList<BoundarySegment>> segs = element.GetBoundarySegments(opt);

                return (segs == null || segs.Count == 0) ? ElementValue.NOT_ENCLOSED : ElementValue.REDUNDANT;
            }
        }
    }

    public enum ElementType
    {
        ROOM,
        AREA
    }

    public enum ElementValue
    {
        NOT_PLACED,
        NOT_ENCLOSED,
        REDUNDANT,
        PLACED
    }
}