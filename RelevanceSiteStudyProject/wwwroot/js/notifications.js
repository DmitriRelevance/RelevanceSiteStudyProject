window.notificationUtils = {
    registerOutsideClickById: function (elementId, dotNetHelper) {
        const element = document.getElementById(elementId);

        if (!element) return;

        function onClick(event) {
            if (!element.contains(event.target)) {
                dotNetHelper.invokeMethodAsync("HidePanel");
                document.removeEventListener("click", onClick);
            }
        }

        setTimeout(() => {
            document.addEventListener("click", onClick);
        }, 100);
    }
};
