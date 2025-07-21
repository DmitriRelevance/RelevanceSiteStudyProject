window.scrollHelper = {
    getScrollInfo: function (elementId) {
        const el = document.getElementById(elementId);
        if (!el) return null;

        var scrollTop = Math.round(el.scrollTop);
        var scrollHeight = Math.round(el.scrollHeight);
        var clientHeight = Math.round(el.clientHeight);

        console.log("scrollTop" + scrollTop)
        console.log("scrollHeight" + scrollHeight)
        console.log("clientHeight" + clientHeight)

        return {
            scrollTop: scrollTop,
            scrollHeight: scrollHeight,
            clientHeight: clientHeight
        };
    }
};