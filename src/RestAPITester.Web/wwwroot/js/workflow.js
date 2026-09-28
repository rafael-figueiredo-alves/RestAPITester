window.getBoundingRect = (el) => {
    const r = el.getBoundingClientRect();
    return { left: r.left, top: r.top };
};