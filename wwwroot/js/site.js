// Mobile menu toggle
const menuBtn = document.getElementById("menuBtn");
const nav = document.getElementById("nav");
if (menuBtn && nav) {
    menuBtn.addEventListener("click", () => nav.classList.toggle("open"));
    nav.querySelectorAll("a").forEach(a => a.addEventListener("click", () => nav.classList.remove("open")));
}

// Open WhatsApp links in the same tab. wa.me lets WhatsApp choose the best app:
// the phone app on mobile, WhatsApp Desktop or WhatsApp Web on a computer.
document.querySelectorAll('a[href^="https://wa.me/"]').forEach(link => {
    link.removeAttribute("target");
    link.removeAttribute("rel");
});

// Smooth reveal of sections on scroll
const revealEls = document.querySelectorAll(".reveal");
if ("IntersectionObserver" in window) {
    const io = new IntersectionObserver(entries => entries.forEach(e => {
        if (e.isIntersecting) { e.target.classList.add("in"); io.unobserve(e.target); }
    }), { threshold: 0.12 });
    revealEls.forEach(el => io.observe(el));
} else {
    revealEls.forEach(el => el.classList.add("in"));
}