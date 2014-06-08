﻿using Sheet.Block.Core;
using Sheet.Block.Model;
using Sheet.Item.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheet
{
    #region IWindow

    public interface IWindow : IDisposable
    {
        void Show();
        bool? ShowDialog();
    }

    #endregion

    #region IView

    public interface IView : IDisposable
    {
        bool Focus();
        bool IsFocused { get; }
    }

    #endregion

    #region Windows

    public interface IMainWindow : IWindow
    {
    }

    #endregion

    #region Views

    public interface ISheetView : IView
    {
    }

    public interface ILibraryView : IView
    {
    }

    public interface ISolutionView : IView
    {
    }

    public interface IDatabaseView : IView
    {
    }

    public interface ITextView : IView
    {
    }

    #endregion

    #region IServiceLocator

    public interface IServiceLocator
    {
        T GetInstance<T>();
    }

    #endregion

    #region IScopeServiceLocator

    public interface IScopeServiceLocator
    {
        T GetInstance<T>();
        void CreateScope();
        void ReleaseScope();
    }
    
    #endregion
    
    #region IClipboard

    public interface IClipboard
    {
        void Set(string text);
        string Get();
    }

    #endregion

    #region IBase64

    public interface IBase64
    {
        string ToBase64(byte[] bytes);
        MemoryStream ToStream(byte[] bytes);
        byte[] ToBytes(string base64);
        MemoryStream ToStream(string base64);
        byte[] ReadAllBytes(string path);
        string FromFileToBase64(string path);
        MemoryStream FromFileToStream(string path);
    }

    #endregion

    #region IJsonSerializer

    public interface IJsonSerializer
    {
        string Serialize(object value);
        T Deerialize<T>(string value);
    }

    #endregion

    #region IEntrySerializer

    public interface IEntrySerializer
    {
        void CreateEmpty(string path);
        void Serialize(SolutionEntry solution, string path);
        SolutionEntry Deserialize(string path);
    }

    #endregion

    #region IEntryController

    public interface IEntryController
    {
        PageEntry AddPage(DocumentEntry document, string content);
        PageEntry AddPageBefore(DocumentEntry document, PageEntry beofore, string content);
        PageEntry AddPageAfter(DocumentEntry document, PageEntry after, string content);
        void AddPageAfter(object item);
        void AddPageBefore(object item);
        void DuplicatePage(object item);
        void RemovePage(object item);

        DocumentEntry AddDocumentBefore(SolutionEntry solution, DocumentEntry after);
        DocumentEntry AddDocumentAfter(SolutionEntry solution, DocumentEntry after);
        DocumentEntry AddDocument(SolutionEntry solution);
        void DocumentAddPage(object item);
        void AddDocumentAfter(object item);
        void AddDocumentBefore(object item);
        void DulicateDocument(object item);
        void RemoveDocument(object item);
    }

    #endregion

    #region IEntryFactory

    public interface IEntryFactory
    {
        PageEntry CreatePage(DocumentEntry document, string content, string name = null);
        DocumentEntry CreateDocument(SolutionEntry solution, string name = null);
    }

    #endregion

    #region IPointController

    public interface IPointController
    {
        void ConnectStart(IPoint point, ILine line);
        void ConnectEnd(IPoint point, ILine line);
        void UpdateDependencies(List<IBlock> blocks, List<IPoint> points, List<ILine> lines);
    }

    #endregion

    #region IPageFactory

    public interface IPageFactory
    {
        void CreateLine(ISheet sheet, IList<ILine> lines, double thickness, double x1, double y1, double x2, double y2, ItemColor stroke);
        void CreateText(ISheet sheet, IList<IText> texts, string content, double x, double y, double width, double height, int halign, int valign, double size, ItemColor foreground);
        void CreateFrame(ISheet sheet, IBlock block, double size, double thickness, ItemColor stroke);
        void CreateGrid(ISheet sheet, IBlock block, double startX, double startY, double width, double height, double size, double thickness, ItemColor stroke);
        
        IRectangle CreateSelectionRectangle(double thickness, double x, double y, double width, double height);
    }

    #endregion

    #region IZoomController

    public interface IZoomController
    {
        int ZoomIndex { get; set; }
        double Zoom { get; set; }
        double PanX { get; set; }
        double PanY { get; set; }
        void AutoFit();
        void ActualSize();
    }

    #endregion

    #region ICursorController

    public enum SheetCursor
    {
        Unknown,
        Normal,
        Move,
        Pan
    }

    public interface ICursorController
    {
        void Set(SheetCursor cursor);
        SheetCursor Get();
    }

    #endregion

    #region IDataReader

    public interface IDataReader
    {
        IEnumerable<string[]> Read(string path);
    }

    #endregion

    #region ISheet

    public interface ISheet
    {
        double Width { get; set; }
        double Height { get; set; }
        bool IsCaptured { get; }
        object GetParent();
        void SetParent(object parent);
        void Add(IElement element);
        void Remove(IElement element);
        void Add(object element);
        void Remove(object element);
        void Capture();
        void ReleaseCapture();
    }

    #endregion

    #region IHistoryController

    public interface IHistoryController
    {
        void Register(string message);
        void Reset();
        void Undo();
        void Redo();
    } 

    #endregion

    #region ILibraryController

    public interface ILibraryController
    {
        BlockItem GetSelected();
        void SetSelected(BlockItem block);
        IEnumerable<BlockItem> GetSource();
        void SetSource(IEnumerable<BlockItem> source);
    } 

    #endregion

    #region IDatabaseController

    public interface IDatabaseController
    {
        string Name { get; set; }
        string[] Columns { get; set; }
        List<string[]> Data { get; set; }
        string[] Get(int index);
        bool Update(int index, string[] item);
        int Add(string[] item);
    }

    #endregion

    #region ITextController

    public interface ITextController
    {
        void Set(Action<string> ok, Action cancel, string title, string label, string text);
    }

    #endregion

    #region ISheetController

    public interface ISheetController
    {
        // Properties
        IHistoryController HistoryController { get; set; }
        ILibraryController LibraryController { get; set; }
        IZoomController ZoomController { get; set; }
        ICursorController CursorController { get; set; }
        SheetOptions Options { get; set; }
        ISheet EditorSheet { get; set; }
        ISheet BackSheet { get; set; }
        ISheet ContentSheet { get; set; }
        ISheet OverlaySheet { get; set; }
        ISheetView View { get; set; }
        double LastFinalWidth { get; set; }
        double LastFinalHeight { get; set; }

        // Blocks
        IBlock GetSelected();
        IBlock GetContent();
        
        // Mode
        SheetMode GetMode();
        void SetMode(SheetMode mode);

        // Init
        void Init();

        // Clipboard
        void CutText();
        void CopyText();
        void PasteText();
        void CutJson();
        void CopyJson();
        void PasteJson();

        // Delete
        void Delete(IBlock block);
        void Delete();

        // Select All
        void SelecteAll();

        // Toggle Fill
        void ToggleFill();

        // Insert Mode
        void CreateBlock();
        void BreakBlock();

        // Move Mode
        void MoveUp();
        void MoveDown();
        void MoveLeft();
        void MoveRight();

        // Pan & Zoom Mode
        void SetAutoFitSize(double finalWidth, double finalHeight);
        void AdjustBackThickness(double zoom);
        void AdjustPageThickness(double zoom);

        // Data Binding
        bool BindDataToBlock(XImmutablePoint p, DataItem dataItem);
        bool BindDataToBlock(IBlock block, DataItem dataItem);
        void TryToBindData(XImmutablePoint p, DataItem dataItem);

        // New Page
        void NewPage();

        // Open Page
        Task OpenTextPage(string path);
        Task OpenJsonPage(string path);
        void OpenPage();

        // Save Page
        void SaveTextPage(string path);
        void SaveJsonPage(string path);
        void SavePage();

        // Export Page
        void Export(IEnumerable<BlockItem> blocks);
        void Export(SolutionEntry solution);
        void ExportPage();

        // Library
        void Insert(XImmutablePoint p);
        IBlock Insert(BlockItem blockItem, XImmutablePoint p, bool select);
        Task LoadLibrary(string path);
        void LoadLibrary();

        // Input
        void LeftDown(InputArgs args);
        void LeftUp(InputArgs args);
        void Move(InputArgs args);
        void RightDown(InputArgs args);
        void RightUp(InputArgs args);
        void Wheel(int delta, XImmutablePoint position);
        void Down(InputArgs args);

        // Page
        void SetPage(string text);
        string GetPage();
        void ExportPage(string text);
        void ExportPages(IEnumerable<string> texts);
        BlockItem SerializePage();
        void DeserializePage(BlockItem page);
        void ResetPage();
        void ResetPageContent();

        // Plugins
        void InvertSelectedLineStart();
        void InvertSelectedLineEnd();
    }

    #endregion
}
