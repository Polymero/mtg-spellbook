
let resizeTimeouts = new Map();


export function observeResize(element, dotNetRef, debounceMs = 100) {
    if (!element || !dotNetRef) return;

    // console.log("ResizeObserver attached", element);

    const observer = new ResizeObserver(entries => {
        for (const entry of entries) {
            const el = entry.target;

            // console.log("Resize event", entry.contentRect.width);

            if (resizeTimeouts.has(el)) {
                clearTimeout(resizeTimeouts.get(el));
            }

            resizeTimeouts.set(el, setTimeout(() => {
                dotNetRef.invokeMethodAsync('OnContainerResize', entry.contentRect.width);
                resizeTimeouts.delete(el);
            }, debounceMs));
        }
    });

    observer.observe(element);


    return {
        dispose: () => {
            observer.disconnect();
            if (resizeTimeouts.has(element)) {
                clearTimeout(resizeTimeouts.get(element));
                resizeTimeouts.delete(element);
            }
        }
    };
}