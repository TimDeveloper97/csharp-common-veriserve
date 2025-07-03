def main(origin_text: str, list_of_words: list ) -> object:
    corrent_text = origin_text
    index_afters = []
    index_befores = []

    for word in list_of_words:
        word_before = word[0]
        word_after = word[1]

        # origin text
        index_before_word = find_word_positions(origin_text, word_before)
  
        # correct text
        corrent_text = corrent_text.replace(word_before, word_after)
        index_after_word = find_word_positions(corrent_text, word_after)

        # append index
        if index_after_word != []:
            index_afters.extend(index_after_word)

        if index_before_word != []:
            index_befores.extend(index_before_word)
    return {
        "origin_text": origin_text,
        "corrent_text": corrent_text,
        "index_befores": index_befores,
        "index_afters": index_afters
    }    



def find_word_positions(origin_text: str, word: str) -> list:
    """Find position word in origin_text"""
    positions = []
    idx = origin_text.find(word)
    while idx != -1:
        pos = [idx + i for i in range(len(word))]
        positions.extend(pos)
        idx = origin_text.find(word, idx + word.__len__())
    return positions

origin_text = "Hello EELL llE, helloLLE! ll llE ELLL"
list_of_words = [
    ["ll", "P"],
    ["E", "K"],
    ["LLE", "Q"],
    ["ELL", "Z"]
]

print(main(origin_text, list_of_words))