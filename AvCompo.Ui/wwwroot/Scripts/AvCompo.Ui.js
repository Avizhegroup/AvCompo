function informElementOutsideClick(dotNetHelper, id) {
    document.addEventListener('click', function (event) {
        var element = document.getElementById(id);
        if (element && !element.contains(event.target)) {
            dotNetHelper.invokeMethodAsync('InformElementOutsideClick');
        }
    });
}

function scrollToHighlightedDropDownItem(containerId, highlightedIndex) {
    const container = document.getElementById(containerId).querySelector('.dropdown');
    const items = container.querySelectorAll('.dropdown-item');
    if (highlightedIndex >= 0 && highlightedIndex < items.length) {
        const highlightedItem = items[highlightedIndex];
        const containerTop = container.scrollTop;
        const containerBottom = containerTop + container.clientHeight;
        const itemTop = highlightedItem.offsetTop;
        const itemBottom = itemTop + highlightedItem.clientHeight;

        if (itemTop < containerTop) {
            container.scrollTop = itemTop;
        } else if (itemBottom > containerBottom) {
            container.scrollTop = itemBottom - container.clientHeight;
        }
    }
}