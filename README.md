# Introduction 
Roslyn analyzer project for the BOSS project, validating specific things that should or should not be used in the BOSS project, and offering code fixes.

# Analyzer rules
1. Error when encountering ```DateTime```, and offer to fix it into ```BossDateTime```