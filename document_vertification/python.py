import re

def main(origin_text: str, list_of_words: list ) -> object:
    
    # find and sort long -> short
    matches = []
    for idx, (word_before, word_after) in enumerate(sorted(list_of_words, key=lambda x: -len(x[0]))):
        for m in re.finditer(re.escape(word_before), origin_text):
            matches.append({
                "start": m.start(),
                "end": m.end(),
                "word_before": word_before,
                "word_after": word_after
            })
    matches.sort(key=lambda x: (x["start"], -len(x["word_before"])))

    print("========================================")
    print(f"matches: {matches}")
    print("========================================")

    # remove overlaps
    used = [False] * len(origin_text)
    filtered = []
    for m in matches:
        if all(not used[i] for i in range(m["start"], m["end"])):
            filtered.append(m)
            for i in range(m["start"], m["end"]):
                used[i] = True

    print("========================================")
    print(f"filtered: {filtered}")
    print("========================================")

    corrent_text = ""
    index_befores = []
    index_afters = []
    pos = 0
    for m in filtered:
        while pos < m["start"]:
            corrent_text += origin_text[pos]
            pos += 1
        
        # insert
        replace_start = len(corrent_text)
        corrent_text += m["word_after"]

        # indexs
        index_befores.extend(range(m["start"], m["end"]))
        index_afters.extend(range(replace_start, replace_start + len(m["word_after"])))
        
        # update pos
        pos = m["end"]

    # insert another
    corrent_text += origin_text[pos:]
        
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
    ["l", "K11"]
]

print(main(origin_text, list_of_words))

# Hello EELL llE, helloLLE! ll llE ELLL
# HePo K11K11LL PK11, hePoLLK11! P PK11 K11LLL
# {
#     'origin_text': 'Hello EELL llE, helloLLE! ll llE ELLL',
#     'corrent_text': 'HePo EK11LL PK11, hePoLLK11! P PK11 K11LLL',
#     'index_befores': [
#         2,3,11,12,18,19,26,27,29,30 | 6,7,13,23,31,33
#     ],
#     'index_afters': [
#         2,14,22,31,33 | 5,6,7, 8,9,10, 15,16,17, 26,27,28, 34,35,36, 38,39,40
#     ]
# }