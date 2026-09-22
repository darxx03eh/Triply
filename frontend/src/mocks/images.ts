const unsplash = (id: string, width = 900) =>
  `https://images.unsplash.com/photo-${id}?auto=format&fit=crop&w=${width}&q=70`

export const stockImages = {
  hero: unsplash('1566073771259-6a8506099945', 1800),
  hotels: [
    unsplash('1542314831-068cd1dbfeeb'),
    unsplash('1551882547-ff40c63fe5fa'),
    unsplash('1520250497591-112f2f40a3f4'),
    unsplash('1571896349842-33c89424de2d'),
    unsplash('1582719478250-c89cae4dc85b'),
    unsplash('1566073771259-6a8506099945'),
  ],
  rooms: [unsplash('1611892440504-42a792e24d32'), unsplash('1590490360182-c33d57733427')],
  cities: {
    Dubai: unsplash('1512453979798-5ea266f8880c'),
    Istanbul: unsplash('1524231757912-21f4fe3a7200'),
    Paris: unsplash('1502602898657-3e91760cbb34'),
    Amman: unsplash('1579606032821-4e6161c81bd3'),
    Jerusalem: unsplash('1552423314-cf29ab68ad73'),
    Aqaba: unsplash('1544551763-46a013bb70d5'),
  } as Record<string, string>,
}
