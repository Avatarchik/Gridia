every((1..3).sample().seconds) {
    range = -1..1
    newLocation = entity.location.add(range.sample(), range.sample(), 0)
    if (walkable(newLocation) || creature(newLocation)) {
        entity.location = newLocation
    }
}
