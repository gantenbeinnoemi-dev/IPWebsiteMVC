// Seitwaerts scrollbare Bereiche: Mausrad -> seitlich, Pfeile links/rechts (am Ende ausgegraut).
document.querySelectorAll('[data-scroller]').forEach(function (scroller) {
    var area = scroller.querySelector('[data-scroll-area]');
    var links = scroller.querySelector('[data-scroll-links]');
    var rechts = scroller.querySelector('[data-scroll-rechts]');
    if (!area || !links || !rechts) return;

    function aktualisieren() {
        var max = area.scrollWidth - area.clientWidth;
        scroller.toggleAttribute('data-overflow', max > 1);
        links.disabled = area.scrollLeft <= 1;
        rechts.disabled = area.scrollLeft >= max - 1;
    }

    function scrollen(richtung) {
        area.scrollBy({ left: richtung * area.clientWidth * 0.8, behavior: 'smooth' });
    }

    links.addEventListener('click', function () { scrollen(-1); });
    rechts.addEventListener('click', function () { scrollen(1); });
    area.addEventListener('scroll', aktualisieren, { passive: true });

    // Mausrad ueber dem Bereich scrollt seitwaerts. Am Ende der Reihe scrollt die Seite normal weiter.
    area.addEventListener('wheel', function (e) {
        if (Math.abs(e.deltaX) > Math.abs(e.deltaY)) return;
        var max = area.scrollWidth - area.clientWidth;
        if (max <= 1) return;
        var nachRechts = e.deltaY > 0;
        if (nachRechts && area.scrollLeft >= max - 1) return;
        if (!nachRechts && area.scrollLeft <= 0) return;
        e.preventDefault();
        area.scrollLeft += e.deltaMode === 1 ? e.deltaY * 32 : e.deltaY;
    }, { passive: false });

    new ResizeObserver(aktualisieren).observe(area);
    window.addEventListener('load', aktualisieren);
    aktualisieren();
});

// Sicherheitsabfrage vor dem Loeschen: <button data-confirm="..."> oder <form data-confirm="...">
document.addEventListener('submit', function (e) {
    var text = (e.submitter && e.submitter.dataset.confirm) || e.target.dataset.confirm;
    if (text && !window.confirm(text)) e.preventDefault();
});

// Vorschau des gewaehlten Cover-Bildes im Bearbeiten-Formular
document.querySelectorAll('input[type=file][data-vorschau]').forEach(function (input) {
    input.addEventListener('change', function () {
        var img = document.getElementById(input.dataset.vorschau);
        var leer = document.getElementById('cover-leer');
        var datei = input.files && input.files[0];
        if (!img || !datei) return;
        img.src = URL.createObjectURL(datei);
        img.hidden = false;
        if (leer) leer.hidden = true;
    });
});
