import streamlit as st
import pandas as pd
import sqlite3
import matplotlib.pyplot as plt

# Connect to the database
conn = sqlite3.connect("scholarship.db")
df = pd.read_sql("SELECT * FROM StudentGrades", conn)

# Scholarship logic
def calculate_award(row):
    REQUIRED_LANGUAGES = {"English 30-1", "English 30-2", "Franais 30-1", "Franais 30-2"}
    REQUIRED_SOCIAL = {"Social Studies 30-1", "Social Studies 30-2"}
    ELIGIBLE_COURSES = REQUIRED_LANGUAGES | REQUIRED_SOCIAL | {
        "Biology 30", "Chemistry 30", "Physics 30", "Science 30",
        "Mathematics 30-1", "Mathematics 30-2", "Mathematics 31",
        "Aboriginal Studies 30", "Language and Culture 30",
        "Art 30", "Drama 30", "Music 30", "Dance 30",
        "Physical Education 30", "Career and Life Management"
    }

    valid_courses = {col: row[col] for col in ELIGIBLE_COURSES if col in row and pd.notna(row[col])}
    lang_course = max((c for c in REQUIRED_LANGUAGES if c in valid_courses), key=lambda c: valid_courses[c], default=None)
    if not lang_course:
        return "Ineligible - No English/French"
    social_course = max((c for c in REQUIRED_SOCIAL if c in valid_courses), key=lambda c: valid_courses[c], default=None)
    if not social_course:
        return "Ineligible - No Social"

    remaining_courses = {k: v for k, v in valid_courses.items() if k not in {lang_course, social_course}}
    top_remaining = sorted(remaining_courses.values(), reverse=True)[:3]

    if len(top_remaining) < 3:
        return "Ineligible - <5 courses"

    marks = [valid_courses[lang_course], valid_courses[social_course]] + top_remaining
    avg = sum(marks) / 5

    if avg >= 80:
        return "$2500"
    elif avg >= 75:
        return "$1500"
    else:
        return "Ineligible - Low Average"

# Apply logic
df["Scholarship Result"] = df.apply(calculate_award, axis=1)

# Title
st.title("🎓 Welcome to Rutherford Scholarship Checker")

# Display student names only
st.subheader("📋 List of Students")
st.write(", ".join(sorted(df["StudentName"].unique())))

# Sidebar filters
st.sidebar.title("Filters")
filter_mode = st.sidebar.radio("Filter by:", ["All", "Grade Level", "Select Names"])

if filter_mode == "All":
    filtered_df = df

elif filter_mode == "Grade Level":
    grade_choice = st.sidebar.selectbox("Choose Grade:", sorted(df["GradeLevel"].unique()))
    filtered_df = df[df["GradeLevel"] == grade_choice]

elif filter_mode == "Select Names":
    names_selected = st.sidebar.multiselect("Select Student(s):", sorted(df["StudentName"].unique()))
    filtered_df = df[df["StudentName"].isin(names_selected)] if names_selected else pd.DataFrame()

# Show results only if filtered_df has content
if not filtered_df.empty:
    st.subheader("🎓 Scholarship Eligibility Results")
    st.dataframe(filtered_df[["StudentName", "GradeLevel", "Scholarship Result"]])

    st.subheader("📊 Summary Charts")
    result_counts = filtered_df["Scholarship Result"].value_counts()
    st.bar_chart(result_counts)

    grade_counts = filtered_df["GradeLevel"].value_counts().sort_index()
    st.bar_chart(grade_counts)

conn.close()
