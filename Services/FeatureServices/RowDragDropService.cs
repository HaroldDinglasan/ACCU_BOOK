using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Accounting_V4.Services.FeatureServices;

public class RowDragDropService<T>
{
    private T? draggedItem;

    public void ConfigureRow(
        RowRenderEventArgs<T> args,
        IList<T> items,
        Action? onDrop = null)
    {
        args.Attributes.Add("title", "Drag row to reorder");
        args.Attributes.Add("style", "cursor:grab");
        args.Attributes.Add("draggable", "true");

        args.Attributes.Add(
            "ondragover",
            "event.preventDefault(); event.target.closest('.rz-data-row')?.classList.add('my-class')"
        );

        args.Attributes.Add(
            "ondragleave",
            "event.target.closest('.rz-data-row')?.classList.remove('my-class')"
        );

        args.Attributes.Add(
            "ondragstart",
            EventCallback.Factory.Create<DragEventArgs>(
                this,
                () =>
                {
                    draggedItem = args.Data;
                })
        );

        args.Attributes.Add(
            "ondrop",
            EventCallback.Factory.Create<DragEventArgs>(
                this,
                () =>
                {
                    Reorder(items, args.Data);

                    onDrop?.Invoke();
                })
        );
    }

    private void Reorder(
        IList<T> items,
        T droppedItem)
    {
        if (draggedItem == null)
            return;

        var draggedIndex = items.IndexOf(draggedItem);
        var droppedIndex = items.IndexOf(droppedItem);

        if (draggedIndex < 0 || droppedIndex < 0)
        {
            draggedItem = default;
            return;
        }

        if (draggedIndex == droppedIndex)
        {
            draggedItem = default;
            return;
        }

        items.RemoveAt(draggedIndex);

        if (draggedIndex < droppedIndex)
            droppedIndex--;

        items.Insert(droppedIndex, draggedItem);

        draggedItem = default;
    }
}
