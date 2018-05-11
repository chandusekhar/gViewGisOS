using System;
using System.Collections.Generic;
using System.Text;
using gView.Framework.UI;
using gView.Framework.Data;
using System.IO;
using gView.DataSources.GDAL;
using gView.Framework.system;
using gView.Framework.system.UI;

namespace gView.DataSources.GDAL.UI
{
    [gView.Framework.system.RegisterPlugIn("BCBE95C6-95C5-432c-8045-918A8B17D270")]
    public class GDALRasterFileExplorerObject : ExplorerObjectCls, IExplorerFileObject, IPlugInDependencies
    {
        private IExplorerIcon _icon = new GDALRasterIcon();
        private string _filename = "";
        private IRasterClass _class = null;

        public GDALRasterFileExplorerObject() : base(null, typeof(RasterClass), 2) { }
        private GDALRasterFileExplorerObject(IExplorerObject parent, string filename)
            : base(parent, typeof(RasterClass), 2)
        {
            _filename = filename;
        }

        #region IExplorerFileObject Members

        public string Filter
        {
            get { return "*.jpg|*.tif|*.tiff|*.dem|*.xpm|w001001.adf|*.png|*.ecw|*.img|*.gsb|*.jp2|*.sid"; }
        }

        public IExplorerIcon Icon
        {
            get
            {
                return _icon;
            }
        }

        #endregion

        #region IExplorerObject Members

        public string Name
        {
            get
            {
                try
                {
                    FileInfo fi = new FileInfo(_filename);
                    return fi.Name;
                }
                catch { return ""; }
            }
        }

        public string FullName
        {
            get { return _filename; }
        }

        public string Type
        {
            get { return "GDAL Raster File"; }
        }

        public void Dispose()
        {
            if (_class != null)
            {
                _class.EndPaint(null);
                _class = null;
            }
        }

        public new object Object
        {
            get
            {
                if (_class == null)
                {
                    try
                    {
                        Dataset dataset = new Dataset();
                        IRasterLayer layer = (IRasterLayer)dataset.AddRasterFile(_filename);

                        if (layer != null && layer.Class is IRasterClass)
                        {
                            _class = layer.Class as IRasterClass;
                            if (_class is RasterClass)
                            {
                                if (!((RasterClass)_class).isValid)
                                {
                                    _class.EndPaint(null);
                                    _class = null;
                                }
                            }
                        }
                    }
                    catch { return _class; }
                }
                return _class;
            }
        }

        public IExplorerObject CreateInstanceByFullName(IExplorerObject parent, string FullName)
        {
            return CreateInstance(parent, FullName);
        }
        public IExplorerFileObject CreateInstance(IExplorerObject parent, string filename)
        {
            try
            {
                if (!(new FileInfo(filename)).Exists) return null;
            }
            catch { return null; }
            return new GDALRasterFileExplorerObject(parent, filename);
        }
        #endregion

        #region ISerializableExplorerObject Member

        public IExplorerObject CreateInstanceByFullName(string FullName, ISerializableExplorerObjectCache cache)
        {
            if (cache.Contains(FullName)) return cache[FullName];

            try
            {
                FileInfo fi = new FileInfo(FullName);
                if (!fi.Exists) return null;

                GDALRasterFileExplorerObject rObject = new GDALRasterFileExplorerObject(null, FullName);
                if (rObject.Object is IRasterClass)
                {
                    cache.Append(rObject);
                    return rObject;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region IPlugInDependencies Member

        public bool HasUnsolvedDependencies()
        {
            return Dataset.hasUnsolvedDependencies;
        }

        #endregion
    }

    internal class GDALRasterIcon : IExplorerIcon
    {
        #region IExplorerIcon Members

        public Guid GUID
        {
            get { return new Guid("687FF527-198D-4597-9DB6-67E48772C070"); }
        }

        public System.Drawing.Image Image
        {
            get { return global::gView.DataSources.GDAL.UI.Properties.Resources.gdal_img; }
        }

        #endregion
    }
}
